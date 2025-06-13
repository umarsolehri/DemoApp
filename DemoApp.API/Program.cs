using Application.Features.Users.Commands;
using Microsoft.AspNetCore.Authorization;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Seeds;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add core services
AddCoreServices(builder.Services, configuration);

// Add authentication and authorization
AddAuthentication(builder.Services, configuration);

// Add Swagger
AddSwagger(builder.Services);

// Add CORS
AddCors(builder.Services, builder.Environment);

// Add MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateUserCommand).Assembly);
});

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        await DatabaseSeeder.SeedAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure middleware pipeline
ConfigureMiddleware(app);

app.Run();




static void AddCoreServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddHttpContextAccessor();
    services.AddExceptionHandler<GlobalExceptionHandler>();

    // Add application services
    services.AddApplication();
    services.AddInfrastructure(configuration);
}

static void AddAuthentication(IServiceCollection services, IConfiguration configuration)
{
    var jwtSettings = configuration.GetSection("Jwt");
    var key = jwtSettings["Key"];
    
    if (string.IsNullOrEmpty(key))
    {
        throw new ArgumentNullException(nameof(key), "JWT Key is not configured in appsettings.json");
    }

    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception is SecurityTokenExpiredException)
                {
                    context.Response.Headers.Add("Token-Expired", "true");
                }
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                // Skip the default logic
                context.HandleResponse();
                return Task.CompletedTask;
            },
            OnMessageReceived = context =>
            {
                // If the request path doesn't match an authorized endpoint, skip token validation
                var endpoint = context.HttpContext.GetEndpoint();
                if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
                {
                    return Task.CompletedTask;
                }
                return Task.CompletedTask;
            }
        };
    });
}

static void AddSwagger(IServiceCollection services)
{
    services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "Mainz.Portal API", Version = "v1" });
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer"
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        Array.Empty<string>()
                    }
                });
    });
}

static void AddCors(IServiceCollection services, IWebHostEnvironment environment)
{
    services.AddCors(options =>
    {
        options.AddPolicy("AllowSpecificOrigin", policy =>
        {
            if (environment.IsDevelopment())
            {
                policy.WithOrigins(
                    "http://localhost:3000",  // Next.js default port
                    "http://localhost:5178",  // API port
                    "https://localhost:7283"  // API HTTPS port
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
            }

        });
    });
}

static void ConfigureMiddleware(WebApplication app)
{
    app.UseExceptionHandler(_ => { });

    // Configure the HTTP request pipeline.
    //if (app.Environment.IsDevelopment())
    //{
    app.UseSwagger();
    app.UseSwaggerUI();
    //}
    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseCors("AllowSpecificOrigin");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
}