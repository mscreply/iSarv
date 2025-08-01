using System.Security.Claims;
using iSarv.Data;
using iSarv.Data.Tests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace iSarv.Areas.Package.Pages
{
    [Authorize]
    public class PrintResultModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public PrintResultModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public TestPackage Package { get; set; }

        public IActionResult OnGet(int id)
        {
            Package = _context.TestPackages.Include(p => p.User)
                .Include(p => p.NeoTest).Include(p => p.CliftonTest)
                .Include(p => p.HollandTest).Include(p => p.RavenTest)
                .FirstOrDefault(p => p.Id == id)!;
            if (Package == null)
            {
                return NotFound();
            }

            if (Package.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier) && !User.IsInRole("Administrator"))
            {
                return Forbid();
            }

            return Page();
        }
    }
}
