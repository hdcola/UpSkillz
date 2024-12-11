using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace UpSkillz.Models;
public class Enrollment
{
    
    [Key]
    public int EnrollmentId { get; set; }

    [Required]
    public Course Course { get; set;} = new Course();
    
    [Required]  
    public IdentityUser Student { get; set; } = new IdentityUser();

    public decimal Amount { get; set; }

    
    [DataType(DataType.DateTime)]	
    public DateTime EnrollmentDate { get; set; } = DateTime.Now;

    [Required]
    public EnrollmentStatus  Status { get; set; } = EnrollmentStatus.Active; 

}

public enum EnrollmentStatus
    {
        Active = 1,
        Completed = 2
    }