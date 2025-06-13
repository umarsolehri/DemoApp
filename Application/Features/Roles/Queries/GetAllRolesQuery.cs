using Application.Abstraction;
using Domain.Contracts.Role;
using Domain.Dtos;
using MediatR;

namespace Application.Features.Roles.Queries;

public record GetAllRolesQuery : IQuery<IEnumerable<RoleDto>>;

public class GetAllRolesQueryHandler : IQueryHandler<GetAllRolesQuery, IEnumerable<RoleDto>>
{
    private readonly IRoleQuery _roleQuery;

    public GetAllRolesQueryHandler(IRoleQuery roleQuery)
    {
        _roleQuery = roleQuery;
    }

    public async Task<IEnumerable<RoleDto>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _roleQuery.GetAllAsync();
        return roles.Select(role => new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description
        });
    }
} 