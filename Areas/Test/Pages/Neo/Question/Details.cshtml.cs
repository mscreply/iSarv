using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using iSarv.Data.Tests;

namespace iSarv.Areas.Test.Pages.Neo.Question
{
    [Authorize(Roles = "Administrator")]
    public class DetailsModel : PageModel
    {
        private readonly Data.ApplicationDbContext _context;

        public DetailsModel(Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public NeoTestQuestion NeoTestQuestion { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Neotestquestion = await _context.NeoTestQuestions.FirstOrDefaultAsync(m => m.Id == id);
            if (Neotestquestion == null)
            {
                return NotFound();
            }
            else
            {
                NeoTestQuestion = Neotestquestion;
            }
            return Page();
        }
    }
}
