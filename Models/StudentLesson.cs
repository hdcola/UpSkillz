using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace UpSkillz.Models;

[Keyless]
public class StudentLesson
{
    [Required]
    public int LessonId { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public Lesson Lesson { get; set; } = null!;

    [Required]
    public User Student { get; set; } = null!;

    public bool IsCompleted { get; set; } = false;
}