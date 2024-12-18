using Microsoft.AspNetCore.Authorization;
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
        private readonly SignInManager<User> _signInManager;

        private ILogger<DashboardController> _logger;

        public DashboardController(ApplicationDbContext context, UserManager<User> userManager, SignInManager<User> signInManager, ILogger<DashboardController> logger)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }
        // GET: Courses     TODO:  for the logged user with the role Instructor or User
        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var courses = await _context.Courses.Where(c => c.Instructor == user).ToListAsync();
                /*
                if (courses == null || !courses.Any())
                {
                    _logger.LogInformation("No courses found in the database.");
                    return View(courses); // Return a view indicating no courses available
                }*/

                return View(courses);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while fetching courses: ", ex);
                return View("Error");
            }
        }

        [Authorize]
        public async Task<IActionResult> SignUpInstructor()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null && !await _userManager.IsInRoleAsync(user, "Instructor"))
            {
                _logger.LogInformation("Sign up for instructor: " + user.UserName);

                // Register User as instructor
                await _userManager.AddToRoleAsync(user, "Instructor");
                // Update cookie so it is aware of new role
                await _signInManager.RefreshSignInAsync(user);
            }

            return RedirectToAction("Index");
        }

        // GET: Dashboard/Course/5
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> Course(int id)
        {
            _logger.LogInformation($"Course {id} Lessons");
            var user = await _userManager.GetUserAsync(User);

            var course = await _context.Courses
                .Include(c => c.Lessons)
                .Where(c => c.CourseId == id)
                .FirstOrDefaultAsync();

            if (course == null || course.Instructor != user)
            {
                _logger.LogInformation("Invalid Instructor");
                return RedirectToAction("Index");
            }

            return View(course);
        }
    }
}
