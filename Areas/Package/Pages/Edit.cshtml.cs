using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace iSarv.Areas.Package.Pages
{
    [Authorize(Roles = "Administrator")]
    public class EditModel : PageModel
    {
        private readonly Data.ApplicationDbContext _context;

        public EditModel(Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public class TestPackageEditModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string UserId { get; set; }
            
            [Display(Name = "Start Date")]
            public DateTime NeoStartDate { get; set; }
            
            [Display(Name = "Deadline")]
            public DateTime NeoDeadline { get; set; }
            
            [Display(Name = "Start Date")]
            public DateTime CliftonStartDate { get; set; }
            
            [Display(Name = "Deadline")]
            public DateTime CliftonDeadline { get; set; }
            
            [Display(Name = "Start Date")]
            public DateTime HollandStartDate { get; set; }
            [Display(Name = "Deadline")]
            public DateTime HollandDeadline { get; set; }
            
            [Display(Name = "Start Date")]
            public DateTime RavenStartDate { get; set; }
            [Display(Name = "Deadline")] 
            public DateTime RavenDeadline { get; set; }

        }

        [BindProperty]
        public TestPackageEditModel TestPackageEdit { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testPackage =  await _context.TestPackages
                .Include(tp=>tp.NeoTest).Include(tp=>tp.CliftonTest)
                .Include(tp=>tp.HollandTest).Include(tp=>tp.RavenTest).FirstOrDefaultAsync(m => m.Id == id);
            if (testPackage == null)
            {
                return NotFound();
            }
            TestPackageEdit = new TestPackageEditModel()
            {
                Id = testPackage.Id,
                Name = testPackage.Name,
                UserId = testPackage.UserId,
                NeoStartDate = testPackage.NeoTest.StartDate,
                NeoDeadline = testPackage.NeoTest.Deadline,
                CliftonStartDate = testPackage.CliftonTest.StartDate,
                CliftonDeadline = testPackage.CliftonTest.Deadline,
                HollandStartDate = testPackage.HollandTest.StartDate,
                HollandDeadline = testPackage.HollandTest.Deadline,
                RavenStartDate = testPackage.RavenTest.StartDate,
                RavenDeadline = testPackage.RavenTest.Deadline,
            };
            ViewData["UserId"] = new SelectList(_context.Users.Select(u => new {u.Id, Name = u.NationalId + ": " + u.FullName})
                , "Id", "Name");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!ModelState.IsValid)
            {
                return await OnGetAsync(id);
            }

            var testPackage =  await _context.TestPackages
                .Include(tp=>tp.NeoTest).Include(tp=>tp.CliftonTest)
                .Include(tp=>tp.HollandTest).Include(tp=>tp.RavenTest)
                .FirstOrDefaultAsync(m => m.Id == TestPackageEdit.Id);
            if (testPackage == null)
            {
                return NotFound();
            }

            testPackage.Name = TestPackageEdit.Name;
            testPackage.UserId = TestPackageEdit.UserId;
            if (TestPackageEdit.NeoStartDate >= TestPackageEdit.NeoDeadline ||
                TestPackageEdit.CliftonStartDate >= TestPackageEdit.CliftonDeadline ||
                TestPackageEdit.HollandStartDate >= TestPackageEdit.HollandDeadline ||
                TestPackageEdit.RavenStartDate >= TestPackageEdit.RavenDeadline)
            {
                ModelState.AddModelError(string.Empty, "Deadline date must be after start date for all tests.");
                return await OnGetAsync(id);
            }

            testPackage.NeoTest.StartDate = TestPackageEdit.NeoStartDate;
            testPackage.NeoTest.Deadline = TestPackageEdit.NeoDeadline;

            testPackage.CliftonTest.StartDate = TestPackageEdit.CliftonStartDate;
            testPackage.CliftonTest.Deadline = TestPackageEdit.CliftonDeadline;

            testPackage.HollandTest.StartDate = TestPackageEdit.HollandStartDate;
            testPackage.HollandTest.Deadline = TestPackageEdit.HollandDeadline;

            testPackage.RavenTest.StartDate = TestPackageEdit.RavenStartDate;
            testPackage.RavenTest.Deadline = TestPackageEdit.RavenDeadline;

            testPackage.StartDate = new List<DateTime>
            {
                testPackage.NeoTest.StartDate, testPackage.CliftonTest.StartDate, testPackage.HollandTest.StartDate,
                testPackage.RavenTest.StartDate
            }.Min();
            testPackage.Deadline = new List<DateTime>
            {
                testPackage.NeoTest.Deadline, testPackage.CliftonTest.Deadline, testPackage.HollandTest.Deadline,
                testPackage.RavenTest.Deadline
            }.Max();

            _context.Attach(testPackage.NeoTest).State = EntityState.Modified;
            _context.Attach(testPackage.CliftonTest).State = EntityState.Modified;
            _context.Attach(testPackage.HollandTest).State = EntityState.Modified;
            _context.Attach(testPackage.RavenTest).State = EntityState.Modified;
            
            _context.Attach(testPackage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestPackageExists(TestPackageEdit.Id))
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

        private bool TestPackageExists(int id)
        {
            return _context.TestPackages.Any(e => e.Id == id);
        }
    }
}
