using Application.Abstraction;
using Domain.Abstraction;
using Domain.Contracts.User;
using Domain.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Queries;

public record GetUserByIdQuery(int Id) : IQuery<UserDto>;

public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserDto>
{
    private readonly IUserQuery _userQuery;

    public GetUserByIdQueryHandler(IUserQuery userQuery, IUnitOfWork unitOfWork)
    {
        _userQuery = userQuery;
    }

    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userQuery.GetByIdAsync(request.Id);

        if (user == null)
        {
            throw new UserNotFoundException(request.Id);
        }

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList(),
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
} 