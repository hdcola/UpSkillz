// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using UpSkillz.Data;
using UpSkillz.Models;

namespace UpSkillz.Areas.Identity.Pages.Account.Manage
{
    public class PurchaseHistoryModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public PurchaseHistoryModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public List<Enrollment> Payments { get; set; } = new List<Enrollment>();

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            Payments = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.Student == user)
                .ToListAsync();

            return Page();
        }
    }
}
