using Domain.Entities;

namespace Domain.Contracts.User;

public interface IUserRepository
{
    Task AddAsync(UserEntity user);
    Task UpdateAsync(UserEntity user);
    Task DeleteAsync(UserEntity user);
} 