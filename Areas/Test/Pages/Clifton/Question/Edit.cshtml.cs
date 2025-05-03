using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using iSarv.Data.Tests;

namespace iSarv.Areas.Test.Pages.Clifton.Question
{
    [Authorize(Roles = "Administrator")]
    public class EditModel : PageModel
    {
        private readonly Data.ApplicationDbContext _context;

        public EditModel(Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CliftonTestQuestion CliftonTestQuestion { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliftontestquestion =  await _context.CliftonTestQuestions.FirstOrDefaultAsync(m => m.Id == id);
            if (cliftontestquestion == null)
            {
                return NotFound();
            }
            CliftonTestQuestion = cliftontestquestion;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(CliftonTestQuestion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CliftonTestQuestionExists(CliftonTestQuestion.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool CliftonTestQuestionExists(int id)
        {
            return _context.CliftonTestQuestions.Any(e => e.Id == id);
        }
    }
}
