using iSarv.Data;
using iSarv.Data.Services;
using iSarv.Data.Tests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NuGet.Protocol;

namespace iSarv.Areas.Test.Pages.Neo
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

        public List<NeoTestQuestion> Questions { get; set; }

        [BindProperty] public Dictionary<int, string> Answers { get; set; }

        public async Task<IActionResult> OnGetAsync(int testId)
        {
            if (!await _userManager.DoesTestBelongToUserAsync(User, testId, "neo"))
            {
                return Forbid();
            }

            // Load questions for the given test ID from the database or service
            Questions = _context.NeoTestQuestions.ToList();

            var neoTest = _context.NeoTests.FirstOrDefault(t => t.Id == testId);
            IsDoneOrExpired = neoTest!.IsCompleted || neoTest.Deadline < DateTime.Now;
            IsNotStarted = neoTest.StartDate > DateTime.Now;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int testId, int[] answers)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var neoTest = _context.NeoTests.FirstOrDefault(ct => ct.Id == testId);
            if (neoTest != null)
            {
                neoTest.Response = string.Join(",", answers);
                neoTest.SubmitDate = DateTime.Now;
                var aiResponse = await _AIService.GetAIReplyForTestAsync(neoTest.CalculateScores().ToJson(), "Neo PI-R");
                neoTest.Result = aiResponse.IsSuccess ? aiResponse.Reply : "Wait for AI";
                _context.NeoTests.Update(neoTest);
                await _context.SaveChangesAsync();
            }

            return Redirect("/User/Dashboard");
        }
    }
}
