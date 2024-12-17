using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UpSkillz.Data;
using UpSkillz.Models;

namespace UpSkillz.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private ILogger<CoursesController> _logger;

        public CoursesController(ApplicationDbContext context, ILogger<CoursesController> logger)
        {
            _context = context;
            _logger = logger;
        }


        // GET: Courses
        public async Task<IActionResult> Index()
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
            var instructor = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == course.Instructor.Id);

            ViewBag.instructorName = instructor?.UserName ?? "Anonymous";
            ViewBag.instructorId = course.Instructor.Id;
            ViewBag.courseId = course.CourseId;

            _logger.LogInformation($"Course instructor: {ViewBag.instructorName}");
            return View(course);
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            ViewBag.InstructorList = new SelectList(_context.Users, "Id", "UserName");
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseId,Title,Description,Price,CreatedAt,UpdatedAt,Instructor")] Course course)
        {
            if (ModelState.IsValid)
            {          
                _logger.LogInformation("ILogger: Model is Valid.");
                course.CreatedAt = DateTime.UtcNow;
                course.UpdatedAt = DateTime.UtcNow;
                _context.Add(course);     
                await _context.SaveChangesAsync();           
                _logger.LogInformation("ILogger: Course was saved to database.");
                return RedirectToAction(nameof(Index));
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
            ViewBag.InstructorList = new SelectList(_context.Users, "Id", "UserName");
            return View(course);
        }

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
