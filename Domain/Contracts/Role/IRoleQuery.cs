using Domain.Entities;

namespace Domain.Contracts.Role;

public interface IRoleQuery
{
    Task<IEnumerable<RoleEntity>> GetAllAsync();
    Task<RoleEntity?> GetByIdAsync(int id);
    Task<RoleEntity?> GetByNameAsync(string name);
} 