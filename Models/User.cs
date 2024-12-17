using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace UpSkillz.Models;

public class User : IdentityUser
{
    public ICollection<StudentLesson> StudentsLessons { get; set; } = new List<StudentLesson>();

}
