using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace UpSkillz.Models;

public class User : IdentityUser 
{

    public List<Lesson> Lessons {get;} = [];

}