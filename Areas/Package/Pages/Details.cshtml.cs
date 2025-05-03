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
        private readonly IAIService _AIService;

        public DetailsModel(Data.ApplicationDbContext context, IAIService aiService)
        {
            _context = context;
            _AIService = aiService;
        }

        public TestPackage TestPackage { get; set; } = default!;
        public string AiError { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testPackage = await _context.TestPackages.Include(tp => tp.User)
                .Include(tp=>tp.NeoTest).Include(tp=>tp.CliftonTest)
                .Include(tp=>tp.HollandsTest).Include(tp=>tp.RavensTest)
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

        public async Task<IActionResult> OnPostEditHollandsResultAsync(int id, string result)
        {
            var hollandsTest = await _context.HollandsTests.FindAsync(id);

            if (hollandsTest == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            hollandsTest.Result = result;
            hollandsTest.IsConfirmed = true;
            _context.Attach(hollandsTest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestPackageExists(hollandsTest.Id))
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

        public async Task<IActionResult> OnPostEditRavensResultAsync(int id, string result)
        {
            var ravensTest = await _context.RavensTests.FindAsync(id);

            if (ravensTest == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            ravensTest.Result = result;
            ravensTest.IsConfirmed = true;
            _context.Attach(ravensTest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestPackageExists(ravensTest.Id))
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
        public async Task<IActionResult> OnPostGetNeoResultFromAIAsync(int id)
        {
            var neoTest = await _context.NeoTests.FindAsync(id);

            if (neoTest == null)
            {
                return NotFound();
            }

            var score = neoTest.CalculateScores();
            var aiResponse = await _AIService.GetAvalAIReplyForTestAsync(score.ToJson(), "Neo PI-R");
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
        public async Task<IActionResult> OnPostGetCliftonResultFromAIAsync(int id)
        {
            var cliftonTest = await _context.CliftonTests.FindAsync(id);

            if (cliftonTest == null)
            {
                return NotFound();
            }

            var score = cliftonTest.CalculateScores();
            var aiResponse = await _AIService.GetAvalAIReplyForTestAsync(score.ToJson(), "Neo PI-R");
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
        public async Task<IActionResult> OnPostGetHollandsResultFromAIAsync(int id)
        {
            var hollandsTest = await _context.HollandsTests.FindAsync(id);

            if (hollandsTest == null)
            {
                return NotFound();
            }

            var score = hollandsTest.CalculateScores();
            var aiResponse = await _AIService.GetAvalAIReplyForTestAsync(score.ToJson(), "Neo PI-R");
            hollandsTest.Result = aiResponse.IsSuccess ? aiResponse.Reply : "Wait for AI";
            AiError = aiResponse.IsSuccess ? "" : aiResponse.Reply;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestPackageExists(hollandsTest.Id))
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
        public async Task<IActionResult> OnPostGetRavensResultFromAIAsync(int id)
        {
            var ravensTest = await _context.RavensTests.FindAsync(id);

            if (ravensTest == null)
            {
                return NotFound();
            }

            var score = ravensTest.CalculateScores();
            var aiResponse = await _AIService.GetAvalAIReplyForTestAsync(score.ToJson(), "Neo PI-R");
            ravensTest.Result = aiResponse.IsSuccess ? aiResponse.Reply : "Wait for AI";
            AiError = aiResponse.IsSuccess ? "" : aiResponse.Reply;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestPackageExists(ravensTest.Id))
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
        
        public async Task<IActionResult> OnPostResetHollandsTestAsync(int id)
        {
            var hollandsTest = await _context.HollandsTests.FindAsync(id);

            if (hollandsTest == null)
            {
                return NotFound();
            }

            hollandsTest.Response = string.Empty;
            hollandsTest.Result = string.Empty;
            if(DateTime.Now > hollandsTest.Deadline)
                hollandsTest.Deadline = DateTime.Now.AddDays(3);
            hollandsTest.IsConfirmed = false;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestPackageExists(hollandsTest.Id))
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

        public async Task<IActionResult> OnPostResetRavensTestAsync(int id)
        {
            var ravensTest = await _context.RavensTests.FindAsync(id);

            if (ravensTest == null)
            {
                return NotFound();
            }

            ravensTest.Response = string.Empty;
            ravensTest.Result = string.Empty;
            if(DateTime.Now > ravensTest.Deadline)
                ravensTest.Deadline = DateTime.Now.AddDays(3);
            ravensTest.IsConfirmed = false;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestPackageExists(ravensTest.Id))
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
