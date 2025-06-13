using BCrypt.Net;
using Domain.Abstraction;
using Domain.Contracts.User;
using Domain.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Application.Features.Users.Commands;

public record UpdateUserCommand(int Id, UpdateUserDto User) : ICommand<Result<UserDto>>;

public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, Result<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserQuery _userQuery;
    private readonly IConfiguration _configuration;

    public UpdateUserCommandHandler(
        IUnitOfWork unitOfWork,
        IUserQuery userQuery,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _userQuery = userQuery;
        _configuration = configuration;
    }

    public async Task<Result<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var user = await _userQuery.GetByIdAsync(request.Id);
            if (user == null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<UserDto>.Failure($"User with ID {request.Id} was not found.");
            }

            // Check if new username already exists (if username is being changed)
            if (!string.IsNullOrEmpty(request.User.Username) && request.User.Username != user.Username)
            {
                var existingUser = await _userQuery.GetByUsernameAsync(request.User.Username);
                if (existingUser != null)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<UserDto>.Failure("Username already exists");
                }
            }

            // Check if new email already exists (if email is being changed)
            if (!string.IsNullOrEmpty(request.User.Email) && request.User.Email != user.Email)
            {
                var existingUser = await _userQuery.GetByEmailAsync(request.User.Email);
                if (existingUser != null)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<UserDto>.Failure("Email already exists");
                }
            }

            // Update basic properties
            if (!string.IsNullOrEmpty(request.User.Username))
            {
                user.Username = request.User.Username;
            }
            if (!string.IsNullOrEmpty(request.User.Email))
            {
                user.Email = request.User.Email;
            }
            if (!string.IsNullOrEmpty(request.User.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(request.User.Password);
            }

            // Update roles if provided
            if (request.User.RoleIds != null && request.User.RoleIds.Any())
            {
                // Remove existing roles
                user.UserRoles.Clear();

                // Add new roles
                foreach (var roleId in request.User.RoleIds)
                {
                    user.UserRoles.Add(new UserRoleEntity
                    {
                        UserId = user.Id,
                        RoleId = roleId,
                        AssignedAt = DateTime.UtcNow
                    });
                }
            }

            user.UpdatedAt = DateTime.UtcNow;

            // Update user in the database
            _unitOfWork.GetContext().Set<UserEntity>().Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Commit the transaction
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // Fetch the updated user with roles
            var updatedUser =  await _userQuery.GetByIdAsync(request.Id);


            if (updatedUser == null)
            {
                return Result<UserDto>.Failure("Failed to retrieve updated user");
            }

            // Return the updated user
            return Result<UserDto>.Success(new UserDto
            {
                Id = updatedUser.Id,
                Username = updatedUser.Username,
                Email = updatedUser.Email,
                Roles = updatedUser.UserRoles.Select(ur => ur.Role.Name).ToList(),
                CreatedAt = updatedUser.CreatedAt,
                UpdatedAt = updatedUser.UpdatedAt
            });
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<UserDto>.Failure($"Failed to update user: {ex.Message}");
        }
    }
} 