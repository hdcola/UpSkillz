using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UpSkillz.Data;
using UpSkillz.Models;

namespace UpSkillz.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private ILogger<CheckoutController> _logger;

        public CheckoutController(ApplicationDbContext context, ILogger<CheckoutController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Courses
        public IActionResult Index()
        {
            return View();
        }

    } 
}