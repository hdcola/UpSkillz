using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace UpSkillz.Models
{
    public class UploadModel
    {
        [Required]
        [Display(Name = "File")]
        public IFormFile File { get; set; }
    }

}