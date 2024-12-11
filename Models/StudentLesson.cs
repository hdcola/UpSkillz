using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace UpSkillz.Models;

[Keyless]
public class StudentLesson
{
    
    [Required] 
    public Lesson Lesson { get; set; }
    
    [Required]  
    public User Student { get; set; } 


   //  public ICollection<Lesson> Lessons { get; set; }     
    // public ICollection<User> Students { get; set; } 

    public List<User> Students {get;} = [];
    public List<Lesson> Lessons {get;} = [];
    public bool IsCompleted { get; set; } = false;

}