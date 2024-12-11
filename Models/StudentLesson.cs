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
    public string UserId { get; set; }

    [Required]
    public Lesson Lesson { get; set; }
    
    [Required]
    public User Student { get; set; }

    public bool IsCompleted { get; set; } = false;
}