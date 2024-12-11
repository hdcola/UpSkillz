
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace UpSkillz.Models;

public class Courses
{
    [Key]
    public int Id { get; set; }

    [Required]  
    public IdentityUser Instructor { get; set; } = new IdentityUser();

    [Required(ErrorMessage = "Course title is required.")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 100 characters.")]
	public string Title { get; set;}  = string.Empty;

    [Required(ErrorMessage = "Course description is required.")]
	public string Description { get; set;} = string.Empty;

    public decimal Price { get; set; } = 0;

    [DataType(DataType.DateTime)]	
    public DateTime CreatedAt { get; set; }

    
    [DataType(DataType.DateTime)]	
    public DateTime UpdatedAt { get; set; }
}
