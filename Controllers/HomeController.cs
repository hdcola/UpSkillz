using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace UpSkillz.Controllers;

public class HomeController : Controller
{
    // GET: /home/
    
    public IActionResult Index()
    {
        return View();
    }
}