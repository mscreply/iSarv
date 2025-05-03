using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using iSarv.Data.Tests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iSarv.Areas.Package.Pages
{
    [Authorize(Roles = "Administrator, Psychologist")]
    public class IndexModel : PageModel
    {
        private readonly Data.ApplicationDbContext _context;

        public IndexModel(Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }
        public string Status { get; set; }

        public IList<TestPackage> TestPackages { get;set; } = default!;
        [TempData] public string ToastMessage { get; set; }

        public async Task OnGetAsync(int currentPage = 1, string searchTerm = "", string status = "")
        {
            CurrentPage = currentPage;
            SearchTerm = searchTerm;
            Status = status;

            TestPackages = _context.TestPackages.Include(tp => tp.User)
                .Include(tp => tp.NeoTest).Include(tp => tp.CliftonTest)
                .Include(tp => tp.HollandsTest).Include(tp => tp.RavensTest)
                .ToList();

            switch (status)
            {
                case "Completed":
                    TestPackages = TestPackages.AsEnumerable().Where(tp => tp.IsCompleted).ToList(); break;
                case "Uncompleted":
                    TestPackages = TestPackages.AsEnumerable().Where(tp => !tp.IsCompleted).ToList(); break;
                default: break;
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                TestPackages = TestPackages.Where(tp =>
                    tp.User.FullName.Contains(searchTerm) || tp.User.NationalId.Contains(searchTerm)).ToList(); 
            }

            TotalPages = (int)Math.Ceiling(TestPackages.Count() / (double)PageSize);

            TestPackages = TestPackages
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            TestPackages = TestPackages.OrderByDescending(tp => tp.StartDate).ToList();
        }
        
        public async Task<IActionResult> OnPostLoadFromExcelAsync(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                ToastMessage = "Please upload a valid Excel file.";
                return RedirectToPage();
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    await excelFile.CopyToAsync(stream);
                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                        {
                            ToastMessage = "The uploaded Excel file contains no worksheets.";
                            return RedirectToPage();
                        }

                        var rowCount = worksheet.RowsUsed().Count();
                        if (rowCount < 2) // Assuming the first row is the header
                        {
                            ToastMessage = "The uploaded Excel file is empty or has no data rows.";
                            return RedirectToPage();
                        }

                        foreach (var row in worksheet.RowsUsed().Skip(1)) // Skip header row
                        {
                            var userName = row.Cell(1).GetString();
                            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
                            if (user != null)
                            {
                                var testPackage = new TestPackage()
                                {
                                    UserId = user.Id
                                };
                                _context.TestPackages.Add(testPackage);
                            }

                            await _context.SaveChangesAsync();
                        }
                    }
                }

                ToastMessage = "Packages loaded successfully from Excel.";
            }
            catch (Exception ex)
            {
                ToastMessage = $"An error occurred while processing the file: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}
