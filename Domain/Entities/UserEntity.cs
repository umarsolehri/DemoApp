using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class UserEntity
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string PasswordHash { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation property for roles
    public ICollection<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();
} 