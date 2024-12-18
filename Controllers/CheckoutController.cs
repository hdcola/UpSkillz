using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Stripe;
using UpSkillz.Data;
using UpSkillz.Models;
using UpSkillz.Services;
using System.Security.Claims;

namespace UpSkillz.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly StripeService _stripeService;
        private readonly string _stripePublicKey;
        private ILogger<CheckoutController> _logger;

        public CheckoutController(ApplicationDbContext context, ILogger<CheckoutController> logger, StripeService stripeService)
        {
            _context = context;
            _logger = logger;
            _stripeService = stripeService;
            _stripePublicKey = Environment.GetEnvironmentVariable("Stripe__PublicKey") ?? string.Empty;
        }

        // GET: Checkout
        public IActionResult Index(string cid)
        {
            if (!int.TryParse(cid, out int courseId))
            {
                _logger.LogError("Invalid course ID: {CourseId}", cid);
                return BadRequest("Invalid course ID");
            }

            _logger.LogInformation("Accessing checkout page for course ID {CourseId}", courseId);
            ViewBag.PublishableKey = _stripePublicKey;
            ViewBag.CourseId = courseId;

            var course = _context.Courses
                .Include(c => c.Instructor)
                .FirstOrDefault(c => c.CourseId == courseId);
            if (course == null)
            {
                _logger.LogError("Course not found for ID: {CourseId}", courseId);
                return NotFound("Course not found");
            }

            var userName = User.Identity?.Name ?? "Guest";
            var email = User.FindFirstValue(ClaimTypes.Email);
            ViewBag.UserName = userName;
            ViewBag.Email = email;

            if (course.Price == 0)
            {
                return View("Free", course);
            }

            return View(course);
        }

        [HttpPost]
        public IActionResult CreatePaymentIntent([FromBody] PaymentModel paymentModel)
        {
            try
            {
                _logger.LogInformation("Creating payment intent for amount: {Amount}", paymentModel.Amount);
                var paymentIntent = _stripeService.CreatePaymentIntent(paymentModel.Amount);
                _logger.LogInformation("Payment intent created successfully. ID: {PaymentIntentId}", paymentIntent.Id);

                // Get the current user
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var student = _context.Users.FirstOrDefault(u => u.Id == userId);

                // Get the course
                var course = _context.Courses.FirstOrDefault(c => c.CourseId == paymentModel.CourseId);

                if (course == null)
                {
                    _logger.LogError("Course not found for ID: {CourseId}", paymentModel.CourseId);
                    return NotFound("Course not found");
                }

                if (student == null)
                {
                    _logger.LogError("User not found for ID: {UserId}", userId);
                    return NotFound("User not found");
                }

                // Create a new enrollment
                var enrollment = new Enrollment
                {
                    Student = student,
                    Course = course,
                    Amount = course.Price, // Convert cents to decimal
                    EnrollmentDate = DateTime.Now,
                    Status = EnrollmentStatus.Active,
                    PaymentReference = paymentIntent.Id
                };

                // Save the enrollment to the database
                _context.Enrollments.Add(enrollment);
                _context.SaveChanges();

                return Json(new { clientSecret = paymentIntent.ClientSecret });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment intent for amount: {Amount}", paymentModel.Amount);
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult ProcessFreeEnrollment([FromBody] EnrollmentModel enrollmentModel)
        {
            try
            {
                // Get the current user
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var student = _context.Users.FirstOrDefault(u => u.Id == userId);

                // Get the course
                var course = _context.Courses.FirstOrDefault(c => c.CourseId == enrollmentModel.CourseId);

                if (course == null)
                {
                    _logger.LogError("Course not found for ID: {CourseId}", enrollmentModel.CourseId);
                    return NotFound("Course not found");
                }

                if (student == null)
                {
                    _logger.LogError("User not found for ID: {UserId}", userId);
                    return NotFound("User not found");
                }

                // Create a new enrollment
                var enrollment = new Enrollment
                {
                    Student = student,
                    Course = course,
                    Amount = 0, // Free course
                    EnrollmentDate = DateTime.Now,
                    Status = EnrollmentStatus.Active,
                    PaymentReference = "Free"
                };

                // Save the enrollment to the database
                _context.Enrollments.Add(enrollment);
                _context.SaveChanges();

                return Ok(new { message = "Enrollment successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing free enrollment for course ID: {CourseId}", enrollmentModel.CourseId);
                return BadRequest(new { error = ex.Message });
            }
        }

    }
}