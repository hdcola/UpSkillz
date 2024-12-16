using System.ComponentModel.DataAnnotations;
namespace UpSkillz.Models;
public class Lesson
{
    [Key]
    public int LessonId { get; set; }

    [Required]
    public Course Course { get; set; } = new Course();

    [Required(ErrorMessage = "Lesson title is required.")]
    [StringLength(100, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Lesson content is required.")]
    public string Content { get; set; } = string.Empty;

    [StringLength(255)]
    public string VideoUrl { get; set; } = string.Empty;

    public int Duration { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime UpdatedAt { get; set; }

   public ICollection<StudentLesson> StudentsLessons { get; set; } = new List<StudentLesson>();


}

