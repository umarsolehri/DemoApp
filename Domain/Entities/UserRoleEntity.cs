namespace Domain.Entities;

public class UserRoleEntity
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public UserEntity User { get; set; } = null!;
    public RoleEntity Role { get; set; } = null!;
} 