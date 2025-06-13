using Domain.Contracts.User;
using Domain.Dtos;

namespace Application.Account.Queries;

public sealed class VerifyLoginUserQuery : IQuery<AuthResponseDto>
{
    public string Username { get; set; }
    public string Password { get; set; }

    private sealed class VerifyLoginUserQueryHandler : IQueryHandler<VerifyLoginUserQuery, AuthResponseDto>
    {
        private readonly IUserQuery _userQuery;
        private readonly ITokenService _tokenService;

        public VerifyLoginUserQueryHandler(IUserQuery userQuery, ITokenService tokenService)
        {
            _userQuery = userQuery;
            _tokenService = tokenService;
        }

        public async Task<AuthResponseDto> Handle(VerifyLoginUserQuery query, CancellationToken cancellationToken)
        {
            var user = await _userQuery.GetByEmailAsync(query.Username);

            if (user == null || !BCrypt.Net.BCrypt.EnhancedVerify(query.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            var loginVm = new LoggedInUserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
            };

            var token = _tokenService.GenerateToken(loginVm);

            return new AuthResponseDto(
                Token: token,
                Username: user.Username,
                Roles: user.UserRoles.Select(ur => ur.Role.Name).ToList()
            );
        }
    }
}
