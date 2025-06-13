using Domain.Dtos;

namespace Domain.Contracts.Authentication
{
    public interface ITokenService
    {
        string GenerateToken(LoggedInUserDto user);
    }
}
