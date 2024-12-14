using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UpSkillz.Data;
using UpSkillz.Models;

namespace UpSkillz.Controllers
{
    public class LessonsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private ILogger<LessonsController> _logger;

        public LessonsController(ApplicationDbContext context, ILogger<LessonsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Lessons
        public async Task<IActionResult> Index()
        {
            return View(await _context.Lessons.ToListAsync());
        }

        // GET: Lessons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lessons
                .FirstOrDefaultAsync(m => m.LessonId == id);
            if (lesson == null)
            {
                return NotFound();
            }

            return View(lesson);
        }

        // GET: Lessons/Create?courseId=5
        public async Task<IActionResult> Create(int? courseId)
        {
            if (courseId.HasValue)
            {
                var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId);
                if (course == null)
                {
                    _logger.LogInformation("ILogger: course doesn't exist");
                    return NotFound();  
                }
                
                _logger.LogInformation($"ILogger: coursId: {courseId}");
                // Pass course info to the view
                ViewBag.Course = course;
            }
            return View();

        }

        // POST: Lessons/Create?courseId=5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LessonId,Title,Content")] Lesson lesson, int courseId)
        {
            _logger.LogInformation("ILogger: Start creating lesson");
            if (ModelState.IsValid)
            {                
                _logger.LogInformation("ILogger: Model is Valid."); 
                var existingCourse = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId);

                if (existingCourse == null)
                {
                    _logger.LogWarning($"No course found with ID {courseId}");
                    return View(lesson);
                }
                
                    lesson.Course = existingCourse;
                    lesson.Course.Description = existingCourse.Description;
                    lesson.CreatedAt = DateTime.UtcNow;
                    lesson.UpdatedAt = DateTime.UtcNow;

                    _context.Add(lesson);
                    await _context.SaveChangesAsync();
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
            
            ViewBag.CoursesList = new SelectList(_context.Courses, "CourseId", "Title", "Description");

            return View(lesson);
        }

        // GET: Lessons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null)
            {
                return NotFound();
            }
            return View(lesson);
        }

        // POST: Lessons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LessonId,Title,Content,VideoUrl,Duration,CreatedAt,UpdatedAt")] Lesson lesson)
        {
            if (id != lesson.LessonId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lesson);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LessonExists(lesson.LessonId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(lesson);
        }

        // GET: Lessons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lessons
                .FirstOrDefaultAsync(m => m.LessonId == id);
            if (lesson == null)
            {
                return NotFound();
            }

            return View(lesson);
        }

        // POST: Lessons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson != null)
            {
                _context.Lessons.Remove(lesson);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LessonExists(int id)
        {
            return _context.Lessons.Any(e => e.LessonId == id);
        }
    }
}
