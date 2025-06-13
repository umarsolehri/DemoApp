using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seeds;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        try
        {
            // Seed roles if they don't exist
            if (!await context.Roles.AnyAsync())
            {
                await context.Roles.AddRangeAsync(
                    new RoleEntity { Id = 1, Name = "Admin", Description = "Administrator with full access" },
                    new RoleEntity { Id = 2, Name = "User", Description = "Regular user with limited access" }
                );
                await context.SaveChangesAsync();
            }

            // Seed admin user if it doesn't exist
            if (!await context.Users.AnyAsync(u => u.Username == "admin"))
            {
                var adminUser = new UserEntity
                {
                    Username = "admin",
                    Email = "admin@gmail.com",
                    PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword("12345"),
                    CreatedAt = DateTime.UtcNow
                };

                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();

                // Add admin role to the user
                var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
                if (adminRole != null)
                {
                    await context.UserRoles.AddAsync(new UserRoleEntity
                    {
                        UserId = adminUser.Id,
                        RoleId = adminRole.Id,
                        AssignedAt = DateTime.UtcNow
                    });
                    await context.SaveChangesAsync();
                }
            }
        }
        catch (Exception ex)
        {

        }
    }
}