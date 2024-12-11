using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UpSkillz.Models;

namespace UpSkillz.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<Course> Courses { get; set; }
    public DbSet<Enrollment> Enrollments  { get; set; }
    public DbSet<Lesson> Lessons  { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}
