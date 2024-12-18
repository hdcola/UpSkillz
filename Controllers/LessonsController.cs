using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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

        private readonly UserManager<User> _userManager;
        private ILogger<LessonsController> _logger;

        public LessonsController(ApplicationDbContext context, UserManager<User> userManager, ILogger<LessonsController> logger)
        {
            _context = context;
            _userManager = userManager;
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
                    // var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId);

                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    _logger.LogInformation($"User Id: {userId}");

                    var enrollment = await _context.Enrollments
                        .Include(e => e.Course)
                        .FirstOrDefaultAsync(e => e.Course.CourseId == courseId && e.Student.Id == userId);

                    if (enrollment == null)
                    {
                        _logger.LogInformation("User is not enrolled in the course or course does not exist.");
                        return View(Enumerable.Empty<Lesson>());
                    }

                    _logger.LogInformation($"User is enrolled in course: {enrollment.Course.Title}");
                    ViewBag.courseId = enrollment.Course.CourseId;
                    ViewBag.courseTitle = enrollment.Course.Title;

                    var lessons = await _context.Lessons
                        .Include(l => l.StudentsLessons.Where(sl => sl.UserId == userId))
                        .Where(l => l.Course.CourseId == enrollment.Course.CourseId)
                        .ToListAsync();

                    return View(lessons);

                }

                _logger.LogWarning("Invalid or missing courseId.");
                ViewBag.ErrorMessage = "Invalid or missing courseId.";
                ViewBag.courseId = null;
                return View(Enumerable.Empty<Lesson>());

            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception: {ex.Message}", ex);
                ViewBag.ErrorMessage = "An unexpected error occurred.";
                return View(Enumerable.Empty<Lesson>());
            }
        }


        // POST: Lessons/CompleteLesson/{id}
        [HttpPost("Lessons/CompleteLesson/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteLesson(int id)
        {
            try
            {
                _logger.LogInformation($"lesson Id: {id}");
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _logger.LogInformation($"userId: {userId}");

                if (userId == null)
                {
                    return BadRequest("User is not authenticated.");
                }

                var lesson = await _context.Lessons
                    .Include(l => l.Course)
                    .FirstOrDefaultAsync(l => l.LessonId == id);

                if (lesson == null)
                {
                    _logger.LogWarning($"Lesson with ID {id} not found.");
                    return NotFound();
                }

                var course = lesson.Course;
                _logger.LogInformation($"courseId: {course.CourseId}");
                ViewBag.courseId = course.CourseId;


                var studentLesson = await _context.StudentsLessons
                    .FirstOrDefaultAsync(sl => sl.LessonId == id && sl.UserId == userId);

                _logger.LogInformation($"studentLesson: {studentLesson}");

                if (studentLesson == null)
                {
                    studentLesson = new StudentLesson
                    {
                        LessonId = id,
                        UserId = userId,
                        IsCompleted = true
                    };

                    _context.StudentsLessons.Add(studentLesson);
                }
                else
                {
                    // Mark it as completed if it already exists
                    studentLesson.IsCompleted = true;
                }

                // Save the changes to the database
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", new { courseId = ViewBag.courseId });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error completing the lesson: {ex.Message}");
                return RedirectToAction("Index", new { ErrorMessage = "An error occurred while completing the lesson." });
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


        /*
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

                if (ModelState.IsValid)
                {
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

            }
            catch (Exception ex)
            {
                _logger.LogWarning($"ILogger: exception -> {ex}");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
                return View(lesson);
            }

        }
        */

        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> Lesson(int courseId, int? id)
        {
            var lesson = new Lesson();

            // Get course
            var user = await _userManager.GetUserAsync(User);
            var course = await _context.Courses.FindAsync(courseId);

            if (course == null || course.Instructor != user)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                _logger.LogInformation($"Modifying course {courseId}");
                lesson.Course = course;
            }

            if (id != null)
            {
                var found = await _context.Lessons
                    .Include(l => l.Course)
                    .FirstOrDefaultAsync(l => l.LessonId == id);

                if (found != null && found.Course.Instructor == user)
                {
                    _logger.LogInformation("Found existing lesson");
                    lesson = found;
                    ViewBag.EditMode = true;
                }
            }

            return View(lesson);
        }

        // POST: Lessons/Lesson
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> Create([Bind("Title,Content,VideoUrl,Duration,Course")] Lesson lesson, int courseId)
        {
            // Get the course
            var user = await _userManager.GetUserAsync(User);
            var course = await _context.Courses.FindAsync(courseId);

            if (course == null || course.Instructor != user)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            lesson.Course = course;
            ModelState.Clear();

            if (TryValidateModel(lesson))
            {
                // TODO: Migrate database to support Null
                if (lesson.VideoUrl == null)
                {
                    lesson.VideoUrl = string.Empty;
                }
                _context.Add(lesson);
                await _context.SaveChangesAsync();
                _logger.LogInformation("ILogger: Lesson was saved to database.");

                return RedirectToAction("Course", "Dashboard", new { id = courseId });
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
            return View(lesson);
        }


        // GET: Lessons/Edit/5
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.LessonId == id);

            if (lesson == null || lesson.Course.Instructor != user)
            {
                return NotFound();
            }
            return View(lesson);
        }*/

        // POST: Lessons/Lesson/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> Edit([Bind("LessonId,Title,Content,VideoUrl,Duration")] Lesson lesson, int courseId)
        {
            var user = await _userManager.GetUserAsync(User);
            var course = await _context.Courses.FindAsync(courseId);

            if (course == null || course.Instructor != user)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            lesson.Course = course;
            ModelState.Clear();

            if (TryValidateModel(lesson))
            {
                // TODO: Migrate database to support Null
                if (lesson.VideoUrl == null)
                {
                    lesson.VideoUrl = string.Empty;
                }

                _context.Update(lesson);
                await _context.SaveChangesAsync();

                return RedirectToAction("Course", "Dashboard", new { id = courseId });
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
