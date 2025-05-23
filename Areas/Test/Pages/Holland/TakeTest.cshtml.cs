using iSarv.Data;
using iSarv.Data.Services;
using iSarv.Data.Tests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NuGet.Protocol;

namespace iSarv.Areas.Test.Pages.Holland
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

        public List<HollandTestQuestion> Questions { get; set; }

        [BindProperty]
        public int[] Answers { get; set; }

        [BindProperty] public int CurrentStep { get; set; }
        
        public async Task<IActionResult> OnGetAsync(int testId)
        {
            if (!await _userManager.DoesTestBelongToUserAsync(User, testId, "holland"))
            {
                return Forbid();
            }

            // Load questions for the given test ID from the database or service
            Questions = _context.HollandTestQuestions.ToList();

            var hollandTest = _context.HollandTests.FirstOrDefault(t => t.Id == testId);
            IsDoneOrExpired = hollandTest!.IsCompleted || hollandTest.Deadline < DateTime.Now;
            IsNotStarted = hollandTest.StartDate > DateTime.Now;

            Answers = string.IsNullOrEmpty(hollandTest.Response) ? Enumerable.Repeat(0, Questions.Count()).ToArray() : hollandTest.Response.Split(",").Select(int.Parse).ToArray();
            CurrentStep = Answers.Length > Questions.Count ? Answers[Questions.Count] : 1;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int testId, int[] answers)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var hollandTest = _context.HollandTests.FirstOrDefault(ct => ct.Id == testId);
            if (hollandTest != null)
            {
                hollandTest.Response = string.Join(",", answers);
                hollandTest.SubmitDate = DateTime.Now;
                var aiResponse = await _AIService.GetAIReplyForTestAsync(hollandTest.CalculateScores().ToJson(), "Holland (RIASEC)");
                hollandTest.Result = aiResponse.IsSuccess ? aiResponse.Reply : "Wait for AI";
                _context.HollandTests.Update(hollandTest);
                await _context.SaveChangesAsync();
            }

            return Redirect("/User/Dashboard");
        }

        public async Task<IActionResult> OnPostSaveProgressAsync(int testId, int[] answers)
        {
            var hollandTest = _context.HollandTests.FirstOrDefault(ct => ct.Id == testId);
            if (hollandTest != null)
            {
                hollandTest.Response = string.Join(",", answers.Append(CurrentStep));
                _context.HollandTests.Update(hollandTest);
                await _context.SaveChangesAsync();
                return new JsonResult(new { success = true });
            }

            return new JsonResult(new { success = false });
        }
    }
}
