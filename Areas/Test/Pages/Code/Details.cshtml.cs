using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using iSarv.Data;
using iSarv.Data.Tests;
using Microsoft.AspNetCore.Authorization;

namespace iSarv.Areas.Test.Pages.Code
{
    [Authorize(Roles = "Administrator")]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

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
    }
}
