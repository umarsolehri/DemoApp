using Domain.Abstraction;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Abstraction;

public class UnitOfWork : IUnitOfWork
{
    private readonly IConfiguration _configuration;
    private DbContext _context;
    private IDbContextTransaction _transaction;
    private readonly bool _useInMemoryDb;
    private bool _disposed = false;

    public UnitOfWork(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _context = CreateDbContext(configuration);
        _useInMemoryDb = configuration.GetValue<bool>("UseInMemoryDatabase");
    }

    private static ApplicationDbContext CreateDbContext(IConfiguration configuration)
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
            optionsBuilder.UseSqlServer(connectionString);
        }

        return new ApplicationDbContext(optionsBuilder.Options);
    }

    public DbContext GetContext()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(UnitOfWork));

        if (_context == null)
            throw new InvalidOperationException("Context has not been initialized");

        return _context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_useInMemoryDb)
        {
            // Return a dummy transaction for in-memory database
            return new DummyTransaction();
        }
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        return _transaction;
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (!_useInMemoryDb)
        {
            try
            {
                await _transaction?.CommitAsync(cancellationToken);
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (!_useInMemoryDb)
        {
            try
            {
                await _transaction?.RollbackAsync(cancellationToken);
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _context?.Dispose();
            }
            _transaction = null;
            _context = null;
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~UnitOfWork()
    {
        Dispose(false);
    }

    private class DummyTransaction : IDbContextTransaction
    {
        public Guid TransactionId => Guid.NewGuid();

        public void Dispose() { }
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task RollbackAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Commit() { }
        public void Rollback() { }
    }
}