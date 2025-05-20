using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using iSarv.Data.Tests;
using Microsoft.AspNetCore.Authorization;

namespace iSarv.Areas.Package.Pages
{
    [Authorize(Roles = "Administrator")]
    public class DeleteModel : PageModel
    {
        private readonly Data.ApplicationDbContext _context;

        public DeleteModel(Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TestPackage TestPackage { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testpackage = await _context.TestPackages.Include(tp => tp.User).FirstOrDefaultAsync(m => m.Id == id);

            if (testpackage == null)
            {
                return NotFound();
            }
            else
            {
                TestPackage = testpackage;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testpackage = await _context.TestPackages.FindAsync(id);
            var neoTest = await _context.NeoTests.FindAsync(id);
            var cliftonTest = await _context.CliftonTests.FindAsync(id);
            var hollandTest = await _context.HollandTests.FindAsync(id);
            var ravenTest = await _context.RavenTests.FindAsync(id);
            if (testpackage != null)
            {
                TestPackage = testpackage;
                _context.TestPackages.Remove(TestPackage);
                _context.NeoTests.Remove(neoTest!);
                _context.CliftonTests.Remove(cliftonTest!);
                _context.HollandTests.Remove(hollandTest!);
                _context.RavenTests.Remove(ravenTest!);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
