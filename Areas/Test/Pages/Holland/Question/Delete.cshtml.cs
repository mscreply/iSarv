using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using iSarv.Data.Tests;

namespace iSarv.Areas.Test.Pages.Holland.Question
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
        public HollandTestQuestion HollandTestQuestion { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Hollandtestquestion = await _context.HollandTestQuestions.FirstOrDefaultAsync(m => m.Id == id);

            if (Hollandtestquestion == null)
            {
                return NotFound();
            }
            else
            {
                HollandTestQuestion = Hollandtestquestion;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Hollandtestquestion = await _context.HollandTestQuestions.FindAsync(id);
            if (Hollandtestquestion != null)
            {
                HollandTestQuestion = Hollandtestquestion;
                _context.HollandTestQuestions.Remove(HollandTestQuestion);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
