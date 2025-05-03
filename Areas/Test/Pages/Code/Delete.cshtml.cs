using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using iSarv.Data;
using iSarv.Data.Tests;
using Microsoft.AspNetCore.Authorization;

namespace iSarv.Areas.Test.Pages.Code
{
    [Authorize(Roles = "Administrator")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ActivationCode ActivationCode { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activationcode = await _context.ActivationCodes.FirstOrDefaultAsync(m => m.Id == id);

            if (activationcode == null)
            {
                return NotFound();
            }
            else
            {
                ActivationCode = activationcode;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activationcode = await _context.ActivationCodes.FindAsync(id);
            if (activationcode != null)
            {
                ActivationCode = activationcode;
                _context.ActivationCodes.Remove(ActivationCode);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
