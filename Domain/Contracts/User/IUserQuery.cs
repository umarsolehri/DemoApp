using Domain.Entities;

namespace Domain.Contracts.User;

public interface IUserQuery
{
    Task<UserEntity?> GetByIdAsync(int id);
    Task<UserEntity?> GetByUsernameAsync(string username);
    Task<UserEntity?> GetByEmailAsync(string email);
    Task<IEnumerable<UserEntity>> GetAllAsync();
}