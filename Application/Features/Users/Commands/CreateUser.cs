using BCrypt.Net;
using Domain.Abstraction;
using Domain.Contracts.User;
using Domain.Dtos;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Application.Features.Users.Commands;

public record CreateUserCommand(CreateUserDto User) : ICommand<Result<UserDto>>;

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserQuery _userQuery;
    private readonly IConfiguration _configuration;

    public CreateUserCommandHandler(IUnitOfWork unitOfWork, IUserQuery userQuery, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _userQuery = userQuery;
        _configuration = configuration;
    }

    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Check if username already exists
            var existingUser = await _userQuery.GetByUsernameAsync(request.User.Username);
            if (existingUser != null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<UserDto>.Failure("Username already exists");
            }

            // Check if email already exists
            existingUser = await _userQuery.GetByEmailAsync(request.User.Email);
            if (existingUser != null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<UserDto>.Failure("Email already exists");
            }

            // Create new user
            var user = new UserEntity
            {
                Username = request.User.Username,
                Email = request.User.Email,
                PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(request.User.Password)
            };

            // Add user roles
            foreach (var roleId in request.User.RoleIds)
            {
                user.UserRoles.Add(new UserRoleEntity
                {
                    UserId = user.Id,
                    RoleId = roleId
                });
            }

            await _unitOfWork.GetContext().Set<UserEntity>().AddAsync(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Commit the transaction
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // Fetch the created user with roles
            var createdUser = await _userQuery.GetByEmailAsync(request.User.Email);
            if (createdUser == null)
            {
                return Result<UserDto>.Failure("Failed to retrieve created user");
            }

            // Return the created user
            return Result<UserDto>.Success(new UserDto
            {
                Id = createdUser.Id,
                Username = createdUser.Username,
                Email = createdUser.Email,
                Roles = createdUser.UserRoles.Select(ur => ur.Role.Name).ToList(),
                CreatedAt = createdUser.CreatedAt,
                UpdatedAt = createdUser.UpdatedAt
            });
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<UserDto>.Failure($"Failed to create user: {ex.Message}");
        }
    }
}