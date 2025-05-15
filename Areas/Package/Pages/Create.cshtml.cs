using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using iSarv.Data.Tests;
using Microsoft.AspNetCore.Authorization;

namespace iSarv.Areas.Package.Pages
{
    [Authorize(Roles = "Administrator")]
    public class CreateModel : PageModel
    {
        private readonly Data.ApplicationDbContext _context;

        public CreateModel(Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["UserId"] = new SelectList(_context.Users.Select(u => new {u.Id, Name = u.NationalId + ": " + u.FullName})
                , "Id", "Name");
            return Page();
        }

        public class TestPackageCreateModel
        {
            [Display(Name = "Package Name")]
            public string Name { get; set; } = "Talent And Strength Package";
            [Display(Name = "User Id")]
            public string UserId { get; set; }
        }
        
        [BindProperty]
        public TestPackageCreateModel TestPackageCreate { get; set; } = new ();

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var testPackage = new TestPackage()
            {
                Name = TestPackageCreate.Name,
                UserId = TestPackageCreate.UserId
            };
            
            _context.TestPackages.Add(testPackage);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
