using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UpSkillz.Data;
using UpSkillz.Models;
using UpSkillz.Services;

namespace UpSkillz.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BlobStorageService _blobStorageService;
        private readonly UserManager<User> _userManager;
        private ILogger<CoursesController> _logger;

        public CoursesController(ApplicationDbContext context, BlobStorageService blobStorageService, UserManager<User> userManager, ILogger<CoursesController> logger)
        {
            _context = context;
            _blobStorageService = blobStorageService;
            _userManager = userManager;
            _logger = logger;
        }


        // GET: Courses
        public async Task<IActionResult> Index()
        {
            var courses = await _context.Courses.ToListAsync();
            return View(courses);
        }

        // GET: Courses/Explore
        public async Task<IActionResult> Explore()
        {
            var courses = await _context.Courses.ToListAsync();
            return View(courses);
        }
        // GET: Cards for partial view
        public async Task<IActionResult> Cards()
        {
            var courses = await _context.Courses.ToListAsync();
            return View(courses);
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Instructor)
                .FirstOrDefaultAsync(m => m.CourseId == id);
            if (course == null)
            {
                return NotFound();
            }
            /*var instructor = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == course.Instructor.Id);*/
            var instructor = course.Instructor;

            ViewBag.instructorName = instructor.UserName ?? "Anonymous";
            ViewBag.instructorId = course.Instructor.Id;
            ViewBag.courseId = course.CourseId;

            _logger.LogInformation($"Course instructor: {ViewBag.instructorName}");
            return View(course);
        }

        // GET: Courses/Create
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> Course(int? id)
        {
            var course = new Course();
            if (id != null)
            {
                var found = await _context.Courses.FindAsync(id);
                var user = await _userManager.GetUserAsync(User);

                if (found.Instructor == user)
                {
                    _logger.LogInformation("Found existing course");
                    course = found;
                    ViewBag.EditMode = true;
                }
            }
            //ViewBag.InstructorList = new SelectList(_context.Users, "Id", "UserName");
            return View(course);
        }

        // POST: Courses/Course
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> Course([Bind("Title,Description,Price,File")] Course course)
        {
            var user = await _userManager.GetUserAsync(User);
            course.Instructor = user;

            ModelState.Clear();

            if (TryValidateModel(course))
            {
                _logger.LogInformation("Creating a course");

                course.CreatedAt = DateTime.UtcNow;
                course.UpdatedAt = DateTime.UtcNow;

                if (course.File != null)
                {
                    var blobUrl = await _blobStorageService.UploadFileAsync(course.File);
                    course.imageUrl = blobUrl;
                }

                _context.Add(course);
                await _context.SaveChangesAsync();
                _logger.LogInformation("ILogger: Course was saved to database.");
                return RedirectToAction("Index", "Dashboard");
            }


            _logger.LogInformation("ILogger: Model is NOT valid.");
            foreach (var state in ModelState)
            {
                var field = state.Key;
                foreach (var error in state.Value.Errors)
                {
                    _logger.LogInformation($"ILogger: Validation Error in field '{field}': {error.ErrorMessage}");
                }
            }
            //ViewBag.InstructorList = new SelectList(_context.Users, "Id", "UserName");
            return View(course);
        }

        // POST: Courses/Course/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> Edit([Bind("CourseId,Title,Description,Price,imageUrl,File,Instructor")] Course course)
        {
            var user = await _userManager.GetUserAsync(User);
            course.Instructor = user;

            ModelState.Clear();

            if (TryValidateModel(course))
            {
                course.UpdatedAt = DateTime.UtcNow;
                _context.Update(course);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Dashboard");
            }

            return View(course);
        }

        /*
        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CourseId,Title,Description,Price,CreatedAt,UpdatedAt,Instructor")] Course course)
        {

            _logger.LogInformation($"ILogger: Trying to update - Title: {course.Title}, Description: {course.Description}, Price: {course.Price}, CreatedAt: {course.CreatedAt}, UpdatedAt: {course.UpdatedAt}");
            if (id != course.CourseId)
            {
                _logger.LogInformation("ILogger: Course is NOT found.");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingCourse = await _context.Courses
                                       .Include(c => c.Instructor)
                                       .FirstOrDefaultAsync(c => c.CourseId == id);

                    if (existingCourse == null)
                    {
                        _logger.LogWarning($"ILogger: Course {id} does not exist in the database.");
                        return NotFound();
                    }

                    _logger.LogInformation($"ILogger: Trying to update Course {existingCourse.CourseId} with Instructor: {existingCourse.Instructor.Id}");

                    existingCourse.Title = course.Title;
                    existingCourse.Description = course.Description;
                    existingCourse.Price = course.Price;
                    existingCourse.CreatedAt = course.CreatedAt;
                    existingCourse.UpdatedAt = course.UpdatedAt;

                    if (course.Instructor != null)
                    {
                        existingCourse.Instructor = course.Instructor;
                    }
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("ILogger: Course is updated.");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError($"ILogger: Update Concurrency error: {ex}");

                    if (!CourseExists(course.CourseId))
                    {
                        _logger.LogWarning($"ILogger: Course with ID {course.CourseId} was not found during concurrency check.");
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogDebug("ILogger: Other error");
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            _logger.LogInformation("ILogger: MOdel is NOT valid.");
            foreach (var state in ModelState)
            {
                var field = state.Key;
                foreach (var error in state.Value.Errors)
                {
                    _logger.LogInformation($"ILogger: Validation Error in field '{field}': {error.ErrorMessage}");
                }
            }
            return View(course);
        }
        */

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .FirstOrDefaultAsync(m => m.CourseId == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.CourseId == id);
        }
    }
}
