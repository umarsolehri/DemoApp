using Domain.Contracts.User;
using Domain.Contracts.Role;
using Domain.Abstraction;
using Infrastructure.Persistence;
using Infrastructure.User;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Role.Queries;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var useInMemoryDb = configuration.GetValue<bool>("UseInMemoryDatabase");
        
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            if (useInMemoryDb)
            {
                options.UseInMemoryDatabase("DemoAppDb");
            }
            else
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            }
        });
        
        // Account services
        services.AddScoped<ITokenService, TokenService>();

        // User services
        services.AddScoped<IUserQuery, UserQuery>();
        services.AddScoped<IUserRepository, UserRepository>();
        
        // Role services
        services.AddScoped<IRoleQuery, RoleQuery>();
        
        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
