using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using iSarv.Data.Tests;

namespace iSarv.Areas.Test.Pages.Holland.Question
{
    [Authorize(Roles = "Administrator")]
    public class IndexModel : PageModel
    {
        private readonly Data.ApplicationDbContext _context;

        public IndexModel(Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<HollandTestQuestion> HollandTestQuestion { get; set; } = default!;

        public async Task OnGetAsync()
        {
            HollandTestQuestion = await _context.HollandTestQuestions.ToListAsync();
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
                            var question = await _context.HollandTestQuestions.FindAsync(id);
                            if (question != null)
                            {
                                question.Statement = row.Cell(2).GetString();
                                question.HollandPersonality =
                                    Enum.TryParse<HollandPersonality>(row.Cell(3).GetString(), out var personality)
                                        ? personality
                                        : default;
                                question.Type =
                                    Enum.TryParse<HollandQuestionType>(row.Cell(4).GetString(), out var type)
                                        ? type
                                        : default;
                                _context.HollandTestQuestions.Update(question);
                            }
                            else
                            {
                                question = new HollandTestQuestion
                                {
                                    Id = row.Cell(1).GetValue<int>(), // Assuming the first column is the ID
                                    Statement = row.Cell(2).GetString(),
                                    HollandPersonality =
                                        Enum.TryParse<HollandPersonality>(row.Cell(3).GetString(), out var personality)
                                            ? personality
                                            : default,
                                    Type =
                                        Enum.TryParse<HollandQuestionType>(row.Cell(4).GetString(), out var type)
                                            ? type
                                            : default
                                };
                                _context.HollandTestQuestions.Add(question);
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