using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using iSarv.Data.Tests;

namespace iSarv.Areas.Test.Pages.Clifton.Question
{
    [Authorize(Roles = "Administrator")]
    public class DetailsModel : PageModel
    {
        private readonly Data.ApplicationDbContext _context;

        public DetailsModel(Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public CliftonTestQuestion CliftonTestQuestion { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliftontestquestion = await _context.CliftonTestQuestions.FirstOrDefaultAsync(m => m.Id == id);
            if (cliftontestquestion == null)
            {
                return NotFound();
            }
            else
            {
                CliftonTestQuestion = cliftontestquestion;
            }
            return Page();
        }
    }
}
