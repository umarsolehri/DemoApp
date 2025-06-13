using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Abstraction;

public abstract class BaseQueryDb<TEntity> where TEntity : class
{
    protected readonly ApplicationDbContext _context;

    protected BaseQueryDb(IConfiguration configuration)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        // Check if we should use in-memory database
        var useInMemoryDb = configuration.GetValue<bool>("UseInMemoryDatabase");
        
        if (useInMemoryDb)
        {
            optionsBuilder.UseInMemoryDatabase("DemoAppDb");
        }
        else
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var readOnlyConnectionString = new SqlConnectionStringBuilder(connectionString)
            {
                ApplicationIntent = ApplicationIntent.ReadOnly
            }.ToString();

            optionsBuilder.UseSqlServer(readOnlyConnectionString);
        }

        _context = new ApplicationDbContext(optionsBuilder.Options);
        _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}

