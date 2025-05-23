using iSarv.Data;
using iSarv.Data.Services;
using iSarv.Data.Tests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NuGet.Protocol;

namespace iSarv.Areas.Test.Pages.Clifton
{
    [Authorize]
    public class TakeTestModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ApplicationUserManager _userManager;
        private IAIService _aiService;

        public TakeTestModel(ApplicationDbContext context, ApplicationUserManager userManager, IAIService aiService)
        {
            _context = context;
            _userManager = userManager;
            _aiService = aiService;
        }

        public bool IsDoneOrExpired { get; set; }
        public bool IsNotStarted { get; set; }

        public List<CliftonTestQuestion> Questions { get; set; }

        [BindProperty]
        public int[] Answers { get; set; }

        [BindProperty] public int CurrentStep { get; set; }

        public async Task<IActionResult> OnGetAsync(int testId)
        {
            if (!await _userManager.DoesTestBelongToUserAsync(User, testId, "clifton"))
            {
                return Forbid();
            }

            // Load questions for the given test ID from the database or service
            Questions = _context.CliftonTestQuestions.ToList();
            
            var cliftonTest = _context.CliftonTests.FirstOrDefault(t => t.Id == testId);
            IsDoneOrExpired = cliftonTest!.IsCompleted || cliftonTest.Deadline < DateTime.Now;
            IsNotStarted = cliftonTest.StartDate > DateTime.Now;

            Answers = string.IsNullOrEmpty(cliftonTest.Response) ? Enumerable.Repeat(2, Questions.Count()).ToArray() : cliftonTest.Response.Split(",").Select(int.Parse).ToArray();
            CurrentStep = Answers.Length > Questions.Count ? Answers[Questions.Count] : 1;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int testId, int[] answers)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var cliftonTest = _context.CliftonTests.FirstOrDefault(ct => ct.Id == testId);
            if (cliftonTest != null)
            {
                cliftonTest.Response = string.Join(",", answers);
                cliftonTest.SubmitDate = DateTime.Now;
                var aiResponse = await _aiService.GetAIReplyForTestAsync(cliftonTest.CalculateScores().ToJson(), "Clifton Strengths");
                cliftonTest.Result = aiResponse.IsSuccess ? aiResponse.Reply : "Wait for AI";
                _context.CliftonTests.Update(cliftonTest);
                await _context.SaveChangesAsync();
            }

            return Redirect("/User/Dashboard");
        }

        public async Task<IActionResult> OnPostSaveProgressAsync(int testId, string[] answers)
        {
            var cliftonTest = _context.CliftonTests.FirstOrDefault(ct => ct.Id == testId);
            if (cliftonTest != null)
            {
                cliftonTest.Response = string.Join(",", answers.Append(CurrentStep.ToString()));
                _context.CliftonTests.Update(cliftonTest);
                await _context.SaveChangesAsync();
                return new JsonResult(new { success = true });
            }

            return new JsonResult(new { success = false });
        }
    }
}
