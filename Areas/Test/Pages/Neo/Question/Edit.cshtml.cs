using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using iSarv.Data.Tests;

namespace iSarv.Areas.Test.Pages.Neo.Question
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
        public NeoTestQuestion NeoTestQuestion { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Neotestquestion =  await _context.NeoTestQuestions.FirstOrDefaultAsync(m => m.Id == id);
            if (Neotestquestion == null)
            {
                return NotFound();
            }
            NeoTestQuestion = Neotestquestion;
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

            _context.Attach(NeoTestQuestion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NeoTestQuestionExists(NeoTestQuestion.Id))
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

        private bool NeoTestQuestionExists(int id)
        {
            return _context.NeoTestQuestions.Any(e => e.Id == id);
        }
    }
}
