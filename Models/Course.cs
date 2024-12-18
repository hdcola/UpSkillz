
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
namespace UpSkillz.Models;

public class Course : UploadModel
{
    [Key]
    public int CourseId { get; set; }

    [Required]
    public User Instructor { get; set; } = null!;

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();


    [Required(ErrorMessage = "Course title is required.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 100 characters.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Course description is required.")]
    public string Description { get; set; } = string.Empty;

    public string imageUrl { get; set; } = string.Empty;


    [Required]
    [Column(TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; } = 0;

    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.Now;


    [DataType(DataType.DateTime)]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

}
