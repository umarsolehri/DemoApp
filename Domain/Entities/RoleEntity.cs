using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class RoleEntity
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    [MaxLength(200)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation property
    public ICollection<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();
} 