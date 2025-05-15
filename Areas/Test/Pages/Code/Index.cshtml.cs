using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using iSarv.Data;
using iSarv.Data.Services;
using iSarv.Data.Tests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq; // Added for SmsService and EmailService

namespace iSarv.Areas.Test.Pages.Code
{
    [Authorize(Roles = "Administrator")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ISmsService _smsService; // Injected SmsService
        private readonly IEmailService _emailService; // Injected EmailService

        public IndexModel(ApplicationDbContext context, ISmsService smsService, IEmailService emailService) // Added SmsService and EmailService to constructor
        {
            _context = context;
            _smsService = smsService;
            _emailService = emailService;
        }

        public IList<ActivationCode> ActivationCode { get; set; } = default!;
        [TempData] public string ToastMessage { get; set; }

        public async Task OnGetAsync()
        {
            ActivationCode = await _context.ActivationCodes.ToListAsync();
        }

        public async Task<IActionResult> OnPostSendSmsAsync(int id) // Added SendSms handler
        {
            var activationCode = await _context.ActivationCodes.FirstOrDefaultAsync(a => a.Id == id);
            if (activationCode == null)
            {
                ToastMessage = "Activation code not found.";
                return RedirectToPage();
            }

            if (string.IsNullOrEmpty(activationCode.PhoneNumber))
            {
                ToastMessage = "Phone number not available for this activation code.";
                return RedirectToPage();
            }

            try
            {
                var response = _smsService.SendSms($"Your package activation code is: {activationCode.Code}",activationCode.PhoneNumber ); // Changed to SendSms and removed await
                ToastMessage = response.IsSuccess ? "Activation code sent via SMS." : response.Result;
            }
            catch (Exception ex)
            {
                ToastMessage = $"Error sending SMS: {ex.Message}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendEmailAsync(int id) // Added SendEmail handler
        {
            var activationCode = await _context.ActivationCodes.FirstOrDefaultAsync(a => a.Id == id);
            if (activationCode == null)
            {
                ToastMessage = "Activation code not found.";
                return RedirectToPage();
            }

            if (string.IsNullOrEmpty(activationCode.Email))
            {
                ToastMessage = "Email address not available for this activation code.";
                return RedirectToPage();
            }

            try
            {
                var result = await _emailService.SendEmailAsync(activationCode.Email, "Your package Activation Code", $"Your activation code is: {activationCode.Code}");
                ToastMessage = result == "Ok" ? "Activation code sent via email." : result;
            }
            catch (Exception ex)
            {
                ToastMessage = $"Error sending email: {ex.Message}";
            }

            return RedirectToPage();
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
                            var id = row.Cell(1).GetValue<int>();
                            var code = new ActivationCode()
                            {
                                Code = string.IsNullOrEmpty(row.Cell(2).GetString())
                                    ? Guid.NewGuid().ToString("N").Substring(0, 8)
                                    : row.Cell(2).GetString(),
                                Email = row.Cell(3).GetValue<string>(),
                                PhoneNumber = row.Cell(4).GetString(),
                                NationalId = row.Cell(5).GetString()
                            };
                            _context.ActivationCodes.Add(code);

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
