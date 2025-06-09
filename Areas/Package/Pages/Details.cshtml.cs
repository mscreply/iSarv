using iSarv.Data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using iSarv.Data.Tests;
using Microsoft.AspNetCore.Authorization;
using NuGet.Protocol;

namespace iSarv.Areas.Package.Pages
{
    [Authorize(Roles = "Administrator, Psychologist")]
    public class DetailsModel : PageModel
    {
        private readonly Data.ApplicationDbContext _context;
        public readonly IAIService AiService;

        public DetailsModel(Data.ApplicationDbContext context, IAIService aiService)
        {
            _context = context;
            AiService = aiService;
        }

        public TestPackage TestPackage { get; set; } = default!;
        [TempData] public string AiError { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testPackage = await _context.TestPackages.Include(tp => tp.User)
                .Include(tp=>tp.NeoTest).Include(tp=>tp.CliftonTest)
                .Include(tp=>tp.HollandTest).Include(tp=>tp.RavenTest)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (testPackage == null)
            {
                return NotFound();
            }
            else
            {
                TestPackage = testPackage;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostEditPackageResultAsync(int id, string result)
        {
            var testPackage = await _context.TestPackages.FindAsync(id);

            if (testPackage == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            testPackage.FinalResult = result;
            _context.Attach(testPackage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestPackageExists(testPackage.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Details", new { id = id });
        }

        public async Task<IActionResult> OnPostEditNeoResultAsync(int id, string result)
        {
            var neoTest = await _context.NeoTests.FindAsync(id);

            if (neoTest == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            neoTest.Result = result;
            neoTest.IsConfirmed = true;
            _context.Attach(neoTest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestPackageExists(neoTest.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Details", new { id = id });
        }

        public async Task<IActionResult> OnPostEditCliftonResultAsync(int id, string result)
        {
            var cliftonTest = await _context.CliftonTests.FindAsync(id);

            if (cliftonTest == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            cliftonTest.Result = result;
            cliftonTest.IsConfirmed = true;
            _context.Attach(cliftonTest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestPackageExists(cliftonTest.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Details", new { id = id });
        }

        public async Task<IActionResult> OnPostEditHollandResultAsync(int id, string result)
        {
            var hollandTest = await _context.HollandTests.FindAsync(id);

            if (hollandTest == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            hollandTest.Result = result;
            hollandTest.IsConfirmed = true;
            _context.Attach(hollandTest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestPackageExists(hollandTest.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Details", new { id = id });
        }

        public async Task<IActionResult> OnPostEditRavenResultAsync(int id, string result)
        {
            var ravenTest = await _context.RavenTests.FindAsync(id);

            if (ravenTest == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            ravenTest.Result = result;
            ravenTest.IsConfirmed = true;
            _context.Attach(ravenTest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestPackageExists(ravenTest.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Details", new { id = id });
        }
        public async Task<IActionResult> OnPostGetPackageResultFromAIAsync(int id)
        {
            var testPackage = await _context.TestPackages.FindAsync(id);

            if (testPackage == null)
            {
                return NotFound();
            }

            // Call the AI service to get the result
            //var aiResult = await _aiService.GetResult(testPackage);

            //testPackage.FinalResult = aiResult;
            //_context.Attach(testPackage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestPackageExists(testPackage.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Details", new { id = id });
        }
        public async Task<IActionResult> OnPostGetNeoResultFromAIAsync(int id, string server, string model)
        {
            var neoTest = await _context.NeoTests.FindAsync(id);

            if (neoTest == null)
            {
                return NotFound();
            }

            var score = neoTest.CalculateScores();
            var aiResponse = await AiService.GetAIReplyForTestAsync(score.ToJson(), "Neo PI-R", server, model);
            neoTest.Result = aiResponse.IsSuccess ? aiResponse.Reply : "Wait for AI";
            AiError = aiResponse.IsSuccess ? "" : aiResponse.Reply;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestPackageExists(neoTest.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Details", new { id = id });
        }
        public async Task<IActionResult> OnPostGetCliftonResultFromAIAsync(int id, string server, string model)
        {
            var cliftonTest = await _context.CliftonTests.FindAsync(id);

            if (cliftonTest == null)
            {
                return NotFound();
            }

            var score = cliftonTest.CalculateScores();
            var aiResponse = await AiService.GetAIReplyForTestAsync(score.ToJson(), "Clifton Strengths (CSTA)", server, model);
            cliftonTest.Result = aiResponse.IsSuccess ? aiResponse.Reply : "Wait for AI";
            AiError = aiResponse.IsSuccess ? "" : aiResponse.Reply;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestPackageExists(cliftonTest.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Details", new { id = id });
        }
        public async Task<IActionResult> OnPostGetHollandResultFromAIAsync(int id, string server, string model)
        {
            var hollandTest = await _context.HollandTests.FindAsync(id);

            if (hollandTest == null)
            {
                return NotFound();
            }

            var score = hollandTest.CalculateScores();
            var aiResponse = await AiService.GetAIReplyForTestAsync(score.ToJson(), "Holland (RIASEC)", server, model);
            hollandTest.Result = aiResponse.IsSuccess ? aiResponse.Reply : "Wait for AI";
            AiError = aiResponse.IsSuccess ? "" : aiResponse.Reply;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestPackageExists(hollandTest.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Details", new { id = id });
        }
        public async Task<IActionResult> OnPostGetRavenResultFromAIAsync(int id, string server, string model)
        {
            var ravenTest = await _context.RavenTests.FindAsync(id);

            if (ravenTest == null)
            {
                return NotFound();
            }

            var score = ravenTest.CalculateScores();
            var aiResponse = await AiService.GetAIReplyForTestAsync(score.ToJson(), "Raven IQ", server, model);
            ravenTest.Result = aiResponse.IsSuccess ? aiResponse.Reply : "Wait for AI";
            AiError = aiResponse.IsSuccess ? "" : aiResponse.Reply;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestPackageExists(ravenTest.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Details", new { id = id });
        }

        public async Task<IActionResult> OnPostResetNeoTestAsync(int id)
        {
            var neoTest = await _context.NeoTests.FindAsync(id);

            if (neoTest == null)
            {
                return NotFound();
            }

            neoTest.Response = string.Empty;
            neoTest.Result = string.Empty;
            if(DateTime.Now > neoTest.Deadline)
                neoTest.Deadline = DateTime.Now.AddDays(3);
            neoTest.IsConfirmed = false;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestPackageExists(neoTest.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Details", new { id = id });
        }
        
        public async Task<IActionResult> OnPostResetCliftonTestAsync(int id)
        {
            var cliftonTest = await _context.CliftonTests.FindAsync(id);

            if (cliftonTest == null)
            {
                return NotFound();
            }

            cliftonTest.Response = string.Empty;
            cliftonTest.Result = string.Empty;
            if(DateTime.Now > cliftonTest.Deadline)
                cliftonTest.Deadline = DateTime.Now.AddDays(3);
            cliftonTest.IsConfirmed = false;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestPackageExists(cliftonTest.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Details", new { id = id });
        }
        
        public async Task<IActionResult> OnPostResetHollandTestAsync(int id)
        {
            var hollandTest = await _context.HollandTests.FindAsync(id);

            if (hollandTest == null)
            {
                return NotFound();
            }

            hollandTest.Response = string.Empty;
            hollandTest.Result = string.Empty;
            if(DateTime.Now > hollandTest.Deadline)
                hollandTest.Deadline = DateTime.Now.AddDays(3);
            hollandTest.IsConfirmed = false;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestPackageExists(hollandTest.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Details", new { id = id });
        }

        public async Task<IActionResult> OnPostResetRavenTestAsync(int id)
        {
            var ravenTest = await _context.RavenTests.FindAsync(id);

            if (ravenTest == null)
            {
                return NotFound();
            }

            ravenTest.Response = string.Empty;
            ravenTest.Result = string.Empty;
            if(DateTime.Now > ravenTest.Deadline)
                ravenTest.Deadline = DateTime.Now.AddDays(3);
            ravenTest.IsConfirmed = false;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestPackageExists(ravenTest.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Details", new { id = id });
        }

        private bool TestPackageExists(int id)
        {
            return _context.TestPackages.Any(e => e.Id == id);
        }
    }
}
