namespace Domain.Dtos;

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public record CreateUserDto(
    string Username,
    string Email,
    string Password,
    List<int> RoleIds
);

public record UpdateUserDto(
    int Id,
    string? Username,
    string? Email,
    string? Password,
    List<int>? RoleIds
);

public record LoginDto(
    string Username,
    string Password
);

public record AuthResponseDto(
    string Token,
    string Username,
    List<string> Roles
); 