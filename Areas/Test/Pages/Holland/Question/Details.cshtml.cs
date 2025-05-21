using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using iSarv.Data.Tests;

namespace iSarv.Areas.Test.Pages.Holland.Question
{
    [Authorize(Roles = "Administrator")]
    public class DetailsModel : PageModel
    {
        private readonly Data.ApplicationDbContext _context;

        public DetailsModel(Data.ApplicationDbContext context)
        {
            _context = context;
        }

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
    }
}
