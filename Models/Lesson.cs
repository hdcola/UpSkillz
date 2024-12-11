using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace UpSkillz.Models;
public class Lesson
{
    [Key]
    public int LessonId { get; set; }

    [Required]  
    public Course Course { get; set; }

    [Required(ErrorMessage = "Lesson title is required.")]
    [StringLength(100, MinimumLength = 1)]
    public string Title { get; set; }

    [Required(ErrorMessage = "Lesson content is required.")]
    public string Content { get; set; }

    [StringLength(255)]
    public string VideoUrl { get; set; }
    public int Duration { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime UpdatedAt { get; set; }

    public ICollection<StudentLesson> StudentsLessons { get; set; }
}

