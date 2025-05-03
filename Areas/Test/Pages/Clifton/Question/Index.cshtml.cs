using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using iSarv.Data.Tests;

namespace iSarv.Areas.Test.Pages.Clifton.Question
{
    [Authorize(Roles = "Administrator")]
    public class IndexModel : PageModel
    {
        private readonly Data.ApplicationDbContext _context;

        public IndexModel(Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<CliftonTestQuestion> CliftonTestQuestion { get; set; } = default!;

        public async Task OnGetAsync()
        {
            CliftonTestQuestion = await _context.CliftonTestQuestions.ToListAsync();
        }

        [TempData] public string ToastMessage { get; set; } = string.Empty;

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
                            var id = row.Cell(1).GetValue<int>();
                            var question = await _context.CliftonTestQuestions.FindAsync(id);
                            if (question != null)
                            {
                                question.StatementA = row.Cell(2).GetString();
                                question.ThemeA =
                                    Enum.TryParse<CliftonTheme>(row.Cell(3).GetString(), out var themeA)
                                        ? themeA
                                        : default;
                                question.StatementB = row.Cell(4).GetString();
                                question.ThemeB =
                                    Enum.TryParse<CliftonTheme>(row.Cell(5).GetString(), out var themeB)
                                        ? themeB
                                        : default;
                                _context.CliftonTestQuestions.Update(question);
                            }
                            else
                            {
                                question = new CliftonTestQuestion
                                {
                                    Id = row.Cell(1).GetValue<int>(), // Assuming the first column is the ID
                                    StatementA = row.Cell(2).GetString(),
                                    ThemeA = Enum.TryParse<CliftonTheme>(row.Cell(3).GetString(), out var themeA)
                                        ? themeA
                                        : default,
                                    StatementB = row.Cell(4).GetString(),
                                    ThemeB = Enum.TryParse<CliftonTheme>(row.Cell(5).GetString(), out var themeB)
                                        ? themeB
                                        : default,
                                };
                                _context.CliftonTestQuestions.Add(question);
                            }

                            await _context.SaveChangesAsync();
                        }
                    }
                }

                ToastMessage = "Questions loaded successfully from Excel.";
            }
            catch (Exception ex)
            {
                ToastMessage = $"An error occurred while processing the file: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}