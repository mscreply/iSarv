using iSarv.Data;
using iSarv.Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NuGet.Protocol;

namespace iSarv.Areas.Test.Pages.Raven
{
    [Authorize]
    public class TakeTestModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ApplicationUserManager _userManager;
        private readonly IAIService _AIService;

        public TakeTestModel(ApplicationDbContext context, ApplicationUserManager userManager, IAIService aiService)
        {
            _context = context;
            _userManager = userManager;
            _AIService = aiService;
        }

        public bool IsDoneOrExpired { get; set; }
        public bool IsNotStarted { get; set; }

        [BindProperty]
        public int[] Answers { get; set; }

        [BindProperty] public int CurrentStep { get; set; }

        public async Task<IActionResult> OnGetAsync(int testId)
        {
            if (!await _userManager.DoesTestBelongToUserAsync(User, testId, "raven"))
            {
                return Forbid();
            }

            var ravenTest = _context.RavenTests.FirstOrDefault(t => t.Id == testId);
            IsDoneOrExpired = ravenTest!.IsCompleted || ravenTest.Deadline < DateTime.Now;
            IsNotStarted = ravenTest.StartDate > DateTime.Now;
            
            Answers = string.IsNullOrEmpty(ravenTest.Response) ? Enumerable.Repeat(1, 60).ToArray() : ravenTest.Response.Split(",").Select(int.Parse).ToArray();
            CurrentStep = Answers.Length > 60 ? Answers[60] : 1;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int testId, int[] answers)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var ravenTest = _context.RavenTests.FirstOrDefault(ct => ct.Id == testId);
            if (ravenTest != null)
            {
                ravenTest.Response = string.Join(",", answers);
                ravenTest.SubmitDate = DateTime.Now;
                var aiResponse = await _AIService.GetAIReplyForTestAsync(ravenTest.CalculateScores().ToJson(), "Raven PI-R");
                ravenTest.Result = aiResponse.IsSuccess ? aiResponse.Reply : "Wait for AI";
                _context.RavenTests.Update(ravenTest);
                await _context.SaveChangesAsync();
            }

            return Redirect("/User/Dashboard");
        }

        public async Task<IActionResult> OnPostSaveProgressAsync(int testId, int[] answers)
        {
            var ravenTest = _context.RavenTests.FirstOrDefault(ct => ct.Id == testId);
            if (ravenTest != null)
            {
                ravenTest.Response = string.Join(",", answers.Append(CurrentStep));
                _context.RavenTests.Update(ravenTest);
                await _context.SaveChangesAsync();
                return new JsonResult(new { success = true });
            }

            return new JsonResult(new { success = false });
        }
    }
}
