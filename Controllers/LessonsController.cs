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
            if (!courseId.HasValue)
            {
                _logger.LogWarning("No courseId provided.");
                return NotFound();
            }
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId.Value);
            if (course == null)
            {
                _logger.LogInformation("Course does not exist.");
                return NotFound("The selected course does not exist.");
            }

            _logger.LogInformation($"CourseId in GET method: {course.CourseId}");
            ViewData["courseId"] = course.CourseId;

            return View();
        }


        // POST: Lessons/Create?courseId=5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LessonId,Title,Content")] Lesson lesson, [FromForm] int courseId)
        {
            _logger.LogInformation("Start creating lesson.");
            _logger.LogInformation($"CourseId in POST method: {courseId}");
           
            if (ModelState.IsValid)
            {
                var existingCourse = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId);
                if (existingCourse == null)
                {
                    _logger.LogWarning($"No course found with ID {courseId}");
                    ModelState.AddModelError("courseId", "The selected course does not exist.");
                    ViewData["courseId"] = courseId;
                    return View(lesson);
                }

                lesson.Course = existingCourse;
                lesson.CreatedAt = DateTime.UtcNow;
                lesson.UpdatedAt = DateTime.UtcNow;

                _context.Add(lesson);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            _logger.LogInformation("Model is NOT valid.");
            ViewData["courseId"] = courseId;
            
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
