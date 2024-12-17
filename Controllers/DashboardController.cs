using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UpSkillz.Data;
using UpSkillz.Models;

namespace UpSkillz.Controllers
{
    public class DashboardController : Controller
    {
       
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        private ILogger<DashboardController> _logger;

        public DashboardController(ApplicationDbContext context, UserManager<User> userManager, ILogger<DashboardController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }
        // GET: Courses     TODO:  for the logged user with the role Instructor or User
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
