using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace UpSkillz.Data;

public static class SeedUsersRoles
{
    public static ModelBuilder Seed(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole("Admin")
            {
                NormalizedName = "ADMIN"
            },
            new IdentityRole("Instructor")
            {
                NormalizedName = "INSTRUCTOR"
            },
            new IdentityRole("User")
            {
                NormalizedName = "USER"
            }
        );
        return modelBuilder;
    }
}
