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
    // Define Enrollment relationships
    modelBuilder.Entity<Enrollment>()
        .HasOne(e => e.Course)
        .WithMany(c => c.Enrollments)
        .OnDelete(DeleteBehavior.NoAction);

    modelBuilder.Entity<Enrollment>()
        .HasOne(e => e.Student)
        .WithMany()
        .OnDelete(DeleteBehavior.NoAction);

    modelBuilder.Entity<StudentLesson>()
        .HasKey(sl => new { sl.LessonId, sl.UserId });

    modelBuilder.Entity<StudentLesson>()
        .HasOne(sl => sl.Lesson)
        .WithMany(l => l.StudentsLessons)
        .HasForeignKey(sl => sl.LessonId)
        .OnDelete(DeleteBehavior.NoAction);  

    modelBuilder.Entity<StudentLesson>()
        .HasOne(sl => sl.Student)
        .WithMany(u => u.StudentsLessons)
        .HasForeignKey(sl => sl.UserId)
        .OnDelete(DeleteBehavior.NoAction);  

    base.OnModelCreating(modelBuilder);
}


}
