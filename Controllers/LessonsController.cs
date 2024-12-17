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


        // helper method to fetch course by ID and save dats to ViewBag
        private async Task<Course?> GetCourseAndSetViewBag(int courseId)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (course != null)
            {
                ViewBag.courseId = course.CourseId;
                ViewBag.courseTitle = course.Title;
                ViewBag.courseDescription = course.Description;
                return course;
            }

            ViewBag.status = "error";
            ViewBag.ErrorMessage = "Course does not exist.";
            _logger.LogInformation($"Course with ID {courseId} does not exist.");
            return null;
        }

        // helper method to parse courseId from query string into integer
        private bool TryGetCourseIdFromQuery(out int courseId)
        {
            if (int.TryParse(Request.Query["courseId"], out courseId))
            {
                _logger.LogInformation($"Received courseId: {courseId}");
                return true;
            }

            _logger.LogWarning("Invalid or missing courseId.");
            ViewBag.ErrorMessage = "Invalid or missing courseId.";
            return false;
        }


        // GET: Lessons?courseId=5
        public async Task<IActionResult> Index()
        {
            try
            {
                if (int.TryParse(Request.Query["courseId"], out int courseId))
                {
                    _logger.LogWarning($"Received courseId: {courseId}");
                    var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId);

                    if (course == null)
                    {
                        ViewBag.status = "error";
                        _logger.LogInformation("Course does not exist.");
                        ViewBag.ErrorMessage = "Course does not exist.";
                        return View();
                    }
                    else
                    {
                        _logger.LogInformation($"CourseId in GET method: {course.CourseId} ->  Title: {course.Title}");
                        ViewBag.courseId = course.CourseId;
                        ViewBag.courseTitle = course.Title;
                        return View(await _context.Lessons.Where(l => l.Course.CourseId == courseId).ToListAsync());
                    }
                }
                else
                {
                    _logger.LogWarning("Invalid or missing courseId.");
                    ViewBag.ErrorMessage = "Invalid or missing courseId.";
                    ViewBag.courseId = null;
                    return View();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception: {ex.Message}", ex);
                ViewBag.ErrorMessage = "An unexpected error occurred.";                
                return View();
            }
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
        [HttpGet("Lessons/Create")]
        public async Task<IActionResult> Create()
        {
            try
            {
                if (int.TryParse(Request.Query["courseId"], out int courseId))
                {
                    _logger.LogWarning($"Received courseId: {courseId}");
                    var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId);

                    if (course == null)
                    {
                        ViewBag.status = "error";
                        _logger.LogInformation("Course does not exist.");
                        ViewBag.ErrorMessage = "Course does not exist.";
                    }
                    else
                    {
                        _logger.LogInformation($"CourseId in GET method: {course.CourseId} ->  Title: {course.Title}");
                        ViewBag.courseId = course.CourseId;
                        ViewBag.courseTitle = course.Title;
                        ViewBag.courseDescription = course.Description;
                    }
                }
                else
                {
                    _logger.LogWarning("Invalid or missing courseId.");
                    ViewBag.ErrorMessage = "Invalid or missing courseId.";
                    ViewBag.courseId = null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception: {ex.Message}", ex);
                ViewBag.ErrorMessage = "An unexpected error occurred.";
            }

            return View();
        }



        // POST: Lessons/Create?courseId=5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.        
        [HttpPost("Lessons/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LessonId,Title,Content,Course")] Lesson lesson, int courseId)
        {
            _logger.LogInformation($"Start creating lesson for course {courseId}");
             
           try 
           {    
                var existingCourse = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId);
                if (existingCourse == null)
                {
                    _logger.LogWarning($"No course found with ID {courseId}");
                    ModelState.AddModelError("courseId", "The selected course does not exist.");
                    // ViewBag.courseId = courseId;
                    return View(lesson);
                }
                _logger.LogInformation($"Course with Id: {existingCourse.CourseId} ->  Title: {existingCourse.Title}");
             
                lesson.Course = existingCourse;
                // lesson.Course.Title = existingCourse.Title;
                // lesson.Course.Description = existingCourse.Description;

                ViewBag.courseId = existingCourse.CourseId;
                ViewBag.courseTitle = existingCourse.Title;
                ViewBag.courseDescription = existingCourse.Description;

                ModelState.Clear();
                TryValidateModel(lesson);

                if (ModelState.IsValid) {       
                    lesson.CreatedAt = DateTime.UtcNow;
                    lesson.UpdatedAt = DateTime.UtcNow;

                    _context.Add(lesson);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), new { courseId = lesson.Course.CourseId });
                }

                _logger.LogInformation("Model is NOT valid.");
                foreach (var key in ModelState.Keys)
                {
                    foreach (var error in ModelState[key].Errors)
                    {
                        _logger.LogInformation($"Key: {key}, Error: {error.ErrorMessage}");
                    }
                }
                ViewBag.courseId = courseId;
                return View(lesson);

           } catch(Exception ex)
           {
                _logger.LogWarning($"ILogger: exception -> {ex}");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
                return View(lesson);
           }
            
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

        private async void GetCourseFromUrl()
        {
             try
            {
                if (int.TryParse(Request.Query["courseId"], out int courseId))
                {
                    _logger.LogWarning($"Received courseId: {courseId}");
                    var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId);

                    if (course == null)
                    {
                        ViewBag.status = "error";
                        _logger.LogInformation("Course does not exist.");
                        ViewBag.ErrorMessage = "Course does not exist.";
                    }
                    else
                    {
                        _logger.LogInformation($"CourseId in GET method: {course.CourseId} ->  Title: {course.Title}; Description: {course.Description}");
                        ViewBag.courseId = course.CourseId;
                        ViewBag.courseTitle = course.Title;
                        ViewBag.courseDescription = course.Description;
                    }
                }
                else
                {
                    _logger.LogWarning("Invalid or missing courseId.");
                    ViewBag.ErrorMessage = "Invalid or missing courseId.";
                    ViewBag.courseId = null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception: {ex.Message}", ex);
                ViewBag.ErrorMessage = "An unexpected error occurred.";
            }
        }
    }
}
