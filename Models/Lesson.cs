using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace UpSkillz.Models;

public class Lesson
{
    
    [Key]
    public int LessonId { get; set; }

    [Required]  
    public Course Course { get; set;} 
   
    [Required(ErrorMessage = "Lesson title is required.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 100 characters.")]
	public string Title { get; set;}  = string.Empty;

    // not required, maybe lesson has no video 
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Video URL must be between 1 and 255 characters.")]
    public string VideoUrl { get; set; } = string.Empty;

    
    [Required(ErrorMessage = "Lesson content is required.")]
	public string Content { get; set;} = string.Empty;

    public int Duration { get; set; }

    
    [DataType(DataType.DateTime)]	
    public DateTime CreatedAt { get; set; }

    
    [DataType(DataType.DateTime)]	
    public DateTime UpdatedAt { get; set; }

    // public ICollection<IdentityUser> Students { get; set; } = new List<IdentityUser>();
    public List<User> Students {get;} = [];


}
