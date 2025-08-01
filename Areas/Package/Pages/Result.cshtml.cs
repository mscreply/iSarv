using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using iSarv.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using iSarv.Data.Tests;
using System.Security.Claims;

namespace iSarv.Areas.Package.Pages
{
    [Authorize]
    public class ResultModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ResultModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public TestPackage TestPackage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TestPackage = await _context.TestPackages.Include(p => p.User).FirstOrDefaultAsync(m => m.Id == id);

            if (TestPackage == null)
            {
                return NotFound();
            }

            if (TestPackage.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier) && !User.IsInRole("Administrator"))
            {
                return Forbid();
            }

            return Page();
        }
    }
}
