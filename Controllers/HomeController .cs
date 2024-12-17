using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UpSkillz.Data;
using UpSkillz.Models;

namespace UpSkillz.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private ILogger<CoursesController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<CoursesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Courses
        public IActionResult Index()
        {
            return View();
        }

    } 
}
