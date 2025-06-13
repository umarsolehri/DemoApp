using Domain.Contracts.Role;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Queries;

public class RoleQuery : IRoleQuery
{
    private readonly ApplicationDbContext _context;

    public RoleQuery(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RoleEntity>> GetAllAsync()
    {
        return await _context.Roles.ToListAsync();
    }

    public async Task<RoleEntity?> GetByIdAsync(int id)
    {
        return await _context.Roles.FindAsync(id);
    }

    public async Task<RoleEntity?> GetByNameAsync(string name)
    {
        return await _context.Roles.FirstOrDefaultAsync(r => r.Name == name);
    }
} 