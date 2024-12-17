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
        private ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Courses
        public async Task<IActionResult> Index()
        {
            try
            {
                var courses = await _context.Courses.ToListAsync();

                if (courses == null || !courses.Any())
                {
                    _logger.LogInformation("No courses found in the database.");
                    return View(); // Return a view indicating no courses available
                }

                return View(courses);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while fetching courses: ", ex);
                return View("Error"); 
            }
        }


    } 
}
