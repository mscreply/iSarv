using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using iSarv.Data;
using iSarv.Data.Tests;
using Microsoft.AspNetCore.Authorization;

namespace iSarv.Areas.Test.Pages.Code
{
    [Authorize(Roles = "Administrator")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ActivationCode = new ActivationCode();
            return Page();
        }

        [BindProperty]
        public ActivationCode ActivationCode { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            // Normalize null values to empty strings
            ActivationCode.Email ??= string.Empty;
            ActivationCode.PhoneNumber ??= string.Empty;
            ActivationCode.NationalId ??= string.Empty;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.ActivationCodes.Add(ActivationCode);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
