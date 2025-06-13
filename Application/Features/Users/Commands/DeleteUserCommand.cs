using Domain.Abstraction;
using Domain.Contracts.User;
using Domain.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Users.Commands;

public record DeleteUserCommand(int Id) : ICommand<Result<bool>>;

public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserQuery _userQuery;

    public DeleteUserCommandHandler(IUnitOfWork unitOfWork, IUserQuery userQuery)
    {
        _unitOfWork = unitOfWork;
        _userQuery = userQuery;
    }

    public async Task<Result<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var user = await _userQuery.GetByIdAsync(request.Id);
            if (user == null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<bool>.Failure($"User with ID {request.Id} was not found.");
            }

            // Remove user roles first (due to foreign key constraints)
            user.UserRoles.Clear();
            _unitOfWork.GetContext().Set<UserEntity>().Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Now remove the user
            _unitOfWork.GetContext().Set<UserEntity>().Remove(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Commit the transaction
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<bool>.Failure($"Failed to delete user: {ex.Message}");
        }
    }
} 