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

        public IList<TestPackage> TestPackages { get; set; } = default!;
        [TempData] public string ToastMessage { get; set; }

        public async Task OnGetAsync(int? currentPage, string? userId, string? status, string? fullName, string? nationalId, string? university, DateTime? startDate, DateTime? endDate, string? neoStatus, string? cliftonStatus, string? hollandStatus, string? ravenStatus)
        {
            CurrentPage = currentPage ?? 1;

            var query = _context.TestPackages.Include(tp => tp.User)
                .Include(tp => tp.NeoTest).Include(tp => tp.CliftonTest)
                .Include(tp => tp.HollandTest).Include(tp => tp.RavenTest)
                .ToList();

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(tp => tp.UserId == userId).ToList();
            }

            if (!string.IsNullOrEmpty(status))
            {
                if (status == "Completed")
                {
                    query = query.Where(tp => tp.IsCompleted).ToList();
                }
                else if (status == "Uncompleted")
                {
                    query = query.Where(tp => !tp.IsCompleted).ToList();
                }
            }

            if (!string.IsNullOrEmpty(fullName))
            {
                query = query.Where(tp => tp.User.FullName.Contains(fullName)).ToList();
            }

            if (!string.IsNullOrEmpty(nationalId))
            {
                query = query.Where(tp => tp.User.NationalId.Contains(nationalId)).ToList();
            }

            if (!string.IsNullOrEmpty(university))
            {
                query = query.Where(tp => tp.User.University.Contains(university)).ToList();
            }

            if (startDate.HasValue)
            {
                query = query.Where(tp => tp.StartDate.Date >= startDate.Value.Date).ToList();
            }

            if (endDate.HasValue)
            {
                query = query.Where(tp => tp.Deadline.Date <= endDate.Value.Date).ToList();
            }

            if (!string.IsNullOrEmpty(neoStatus))
            {
                switch (neoStatus)
                {
                    case "Confirmed":
                        query = query.Where(tp => tp.NeoTest.IsCompleted && tp.NeoTest.IsConfirmed).ToList();
                        break;
                    case "NotConfirmed":
                        query = query.Where(tp => tp.NeoTest.IsCompleted && !tp.NeoTest.IsConfirmed).ToList();
                        break;
                    case "InProgress":
                        query = query.Where(tp =>
                            DateTime.Now >= tp.NeoTest.StartDate && DateTime.Now <= tp.NeoTest.Deadline && !tp.NeoTest.IsCompleted).ToList();
                        break;
                    case "Expired":
                        query = query.Where(tp =>
                            DateTime.Now > tp.NeoTest.Deadline && !tp.NeoTest.IsCompleted).ToList();
                        break;
                    case "NotStarted":
                        query = query.Where(tp =>
                            DateTime.Now < tp.NeoTest.StartDate).ToList();
                        break;
                }
            }

            if (!string.IsNullOrEmpty(cliftonStatus))
            {
                switch (cliftonStatus)
                {
                    case "Confirmed":
                        query = query.Where(tp => tp.CliftonTest.IsCompleted && tp.CliftonTest.IsConfirmed).ToList();
                        break;
                    case "NotConfirmed":
                        query = query.Where(tp => tp.CliftonTest.IsCompleted && !tp.CliftonTest.IsConfirmed).ToList();
                        break;
                    case "InProgress":
                        query = query.Where(tp =>
                            DateTime.Now >= tp.CliftonTest.StartDate && DateTime.Now <= tp.CliftonTest.Deadline && !tp.CliftonTest.IsCompleted).ToList();
                        break;
                    case "Expired":
                        query = query.Where(tp =>
                            DateTime.Now > tp.CliftonTest.Deadline && !tp.CliftonTest.IsCompleted).ToList();
                        break;
                    case "NotStarted":
                        query = query.Where(tp =>
                            DateTime.Now < tp.CliftonTest.StartDate).ToList();
                        break;
                }
            }

            if (!string.IsNullOrEmpty(hollandStatus))
            {
                switch (hollandStatus)
                {
                    case "Confirmed":
                        query = query.Where(tp => tp.HollandTest.IsCompleted && tp.HollandTest.IsConfirmed).ToList();
                        break;
                    case "NotConfirmed":
                        query = query.Where(tp => tp.HollandTest.IsCompleted && !tp.HollandTest.IsConfirmed).ToList();
                        break;
                    case "InProgress":
                        query = query.Where(tp =>
                            DateTime.Now >= tp.HollandTest.StartDate && DateTime.Now <= tp.HollandTest.Deadline && !tp.HollandTest.IsCompleted).ToList();
                        break;
                    case "Expired":
                        query = query.Where(tp =>
                            DateTime.Now > tp.HollandTest.Deadline && !tp.HollandTest.IsCompleted).ToList();
                        break;
                    case "NotStarted":
                        query = query.Where(tp =>
                            DateTime.Now < tp.HollandTest.StartDate).ToList();
                        break;
                }
            }

            if (!string.IsNullOrEmpty(ravenStatus))
            {
                switch (ravenStatus)
                {
                    case "Confirmed":
                        query = query.Where(tp => tp.RavenTest.IsCompleted && tp.RavenTest.IsConfirmed).ToList();
                        break;
                    case "NotConfirmed":
                        query = query.Where(tp => tp.RavenTest.IsCompleted && !tp.RavenTest.IsConfirmed).ToList();
                        break;
                    case "InProgress":
                        query = query.Where(tp =>
                            DateTime.Now >= tp.RavenTest.StartDate && DateTime.Now <= tp.RavenTest.Deadline && !tp.RavenTest.IsCompleted).ToList();
                        break;
                    case "Expired":
                        query = query.Where(tp =>
                            DateTime.Now > tp.RavenTest.Deadline && !tp.RavenTest.IsCompleted).ToList();
                        break;
                    case "NotStarted":
                        query = query.Where(tp =>
                            DateTime.Now < tp.RavenTest.StartDate).ToList();
                        break;
                }
            }

            TotalPages = (int)Math.Ceiling(query.Count() / (double)PageSize);

            TestPackages = query
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize).ToList();
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
                    worksheet.Cell(1, 4).Value = "University";
                    worksheet.Cell(1, 5).Value = "Start Date";
                    worksheet.Cell(1, 6).Value = "End Date";
                    worksheet.Cell(1, 7).Value = "Is Completed";
                    worksheet.Cell(1, 8).Value = "NEO Test Status";
                    worksheet.Cell(1, 9).Value = "Clifton Test Status";
                    worksheet.Cell(1, 10).Value = "Holland Test Status";
                    worksheet.Cell(1, 11).Value = "Raven Test Status";

                    // Add data rows
                    var row = 2;
                    foreach (var package in _context.TestPackages.Include(tp => tp.User)
                                 .Include(tp => tp.NeoTest).Include(tp => tp.CliftonTest)
                                 .Include(tp => tp.HollandTest).Include(tp => tp.RavenTest).ToList())
                    {
                        worksheet.Cell(row, 1).Value = package.User?.UserName;
                        worksheet.Cell(row, 2).Value = package.User?.FullName;
                        worksheet.Cell(row, 3).Value = package.User?.NationalId;
                        worksheet.Cell(row, 4).Value = package.User?.University;
                        worksheet.Cell(row, 5).Value = package.StartDate;
                        worksheet.Cell(row, 6).Value = package.Deadline;
                        worksheet.Cell(row, 7).Value = package.IsCompleted;
                        worksheet.Cell(row, 8).Value = package.NeoTest.Status;
                        worksheet.Cell(row, 9).Value = package.CliftonTest.Status;
                        worksheet.Cell(row, 10).Value = package.HollandTest.Status;
                        worksheet.Cell(row, 11).Value = package.RavenTest.Status;
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
