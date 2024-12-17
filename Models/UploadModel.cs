using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace UpSkillz.Models
{
    public class UploadModel
    {
        [Display(Name = "File")]
        [NotMapped]
        public IFormFile File { get; set; }
    }

}