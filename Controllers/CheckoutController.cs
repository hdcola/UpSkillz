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

            var userName = User.Identity?.Name ?? "Guest";
            var email = User.FindFirstValue(ClaimTypes.Email);
            ViewBag.UserName = userName;
            ViewBag.Email = email;

            return View();
        }

        [HttpPost]
        public IActionResult CreatePaymentIntent([FromBody] PaymentModel paymentModel)
        {
            try
            {
                _logger.LogInformation("Creating payment intent for amount: {Amount}", paymentModel.Amount);
                var paymentIntent = _stripeService.CreatePaymentIntent(paymentModel.Amount);
                _logger.LogInformation("Payment intent created successfully. ID: {PaymentIntentId}", paymentIntent.Id);
                return Json(new { clientSecret = paymentIntent.ClientSecret });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment intent for amount: {Amount}", paymentModel.Amount);
                return BadRequest(new { error = ex.Message });
            }
        }

    }
}