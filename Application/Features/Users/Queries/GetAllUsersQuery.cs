using Application.Abstraction;
using Domain.Contracts.User;
using Domain.Dtos;
using MediatR;

namespace Application.Features.Users.Queries;

public record GetAllUsersQuery : IQuery<IEnumerable<UserDto>>;

public class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, IEnumerable<UserDto>>
{
    private readonly IUserQuery _userQuery;

    public GetAllUsersQueryHandler(IUserQuery userQuery)
    {
        _userQuery = userQuery;
    }

    public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userQuery.GetAllAsync();
        return users.Select(user => new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList(),
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        });
    }
} 