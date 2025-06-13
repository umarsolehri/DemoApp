using Domain.Abstraction;
using Domain.Contracts.User;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.User;

public sealed class UserRepository : IUserRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public UserRepository(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task AddAsync(UserEntity user)
    {
        await _unitOfWork.GetContext().Set<UserEntity>().AddAsync(user);
    }

    public async Task UpdateAsync(UserEntity user)
    {
        _unitOfWork.GetContext().Set<UserEntity>().Update(user);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(UserEntity user)
    {
        _unitOfWork.GetContext().Set<UserEntity>().Remove(user);
        await Task.CompletedTask;
    }
} 