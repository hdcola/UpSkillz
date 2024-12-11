using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UpSkillz.Models;

namespace UpSkillz.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public DbSet<Course> Courses { get; set; }
    public DbSet<Enrollment> Enrollments  { get; set; }
    public DbSet<Lesson> Lessons  { get; set; }
    public DbSet<StudentLesson> StudentsLessons  { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Student)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        
        modelBuilder.Entity<User>()
            .HasMany(e => e.Lessons)
            .WithMany(c => c.Students)
            .UsingEntity("StudentLesson");

        base.OnModelCreating(modelBuilder);
    }

}
