using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using iSarv.Data.Tests;
using iSarv.Data;

using System.Linq;
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

        public IList<TestPackage> TestPackages { get; set; } = default!;
        [TempData] public string ToastMessage { get; set; }

        public async Task OnGetAsync(int currentPage = 1, string searchTerm = "", string status = "")
        {
            CurrentPage = currentPage;
            SearchTerm = searchTerm;
            Status = status;

            TestPackages = _context.TestPackages.Include(tp => tp.User)
                .Include(tp => tp.NeoTest).Include(tp => tp.CliftonTest)
                .Include(tp => tp.HollandTest).Include(tp => tp.RavenTest)
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

        public async Task<IActionResult> OnPostSavePackagesToExcelAsync()
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Packages");

                    // Add header row
                    worksheet.Cell(1, 1).Value = "Username(Mobile)";
                    worksheet.Cell(1, 2).Value = "User Full Name";
                    worksheet.Cell(1, 3).Value = "User National ID";
                    worksheet.Cell(1, 4).Value = "Start Date";
                    worksheet.Cell(1, 5).Value = "End Date";
                    worksheet.Cell(1, 6).Value = "Is Completed";
                    worksheet.Cell(1, 7).Value = "NEO Test Status";
                    worksheet.Cell(1, 8).Value = "Clifton Test Status";
                    worksheet.Cell(1, 9).Value = "Holland Test Status";
                    worksheet.Cell(1, 10).Value = "Raven Test Status";

                    // Add data rows
                    var row = 2;
                    foreach (var package in _context.TestPackages.Include(tp => tp.User)
                                 .Include(tp => tp.NeoTest).Include(tp => tp.CliftonTest)
                                 .Include(tp => tp.HollandTest).Include(tp => tp.RavenTest).ToList())
                    {
                        worksheet.Cell(row, 1).Value = package.User?.UserName;
                        worksheet.Cell(row, 2).Value = package.User?.FullName;
                        worksheet.Cell(row, 3).Value = package.User?.NationalId;
                        worksheet.Cell(row, 4).Value = package.StartDate;
                        worksheet.Cell(row, 5).Value = package.Deadline;
                        worksheet.Cell(row, 6).Value = package.IsCompleted;
                        worksheet.Cell(row, 7).Value = package.NeoTest.Status;
                        worksheet.Cell(row, 8).Value = package.CliftonTest.Status;
                        worksheet.Cell(row, 9).Value = package.HollandTest.Status;
                        worksheet.Cell(row, 10).Value = package.RavenTest.Status;
                        row++;
                    }

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Packages.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                ToastMessage = $"An error occurred while saving the packages: {ex.Message}";
                return RedirectToPage();
            }
        }


    }
}
