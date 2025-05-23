using iSarv.Data;
using iSarv.Data.Services;
using iSarv.Data.Tests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NuGet.Protocol;

namespace iSarv.Areas.Test.Pages.Raven;
[Authorize]
public class Result : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly ApplicationUserManager _userManager;
    private readonly IAIService _AIService;

    public Result(ApplicationDbContext context, ApplicationUserManager userManager, IAIService aiService)
    {
        _context = context;
        _userManager = userManager;
        _AIService = aiService;
    }

    public RavenTest RavenTest { get; set; } = default!;

    public Dictionary<string, int> Score { get; set; }

    public string AIError { get; set; } = "";
    public async Task<IActionResult> OnGetAsync(int testId)
    {
        if (!await _userManager.DoesTestBelongToUserAsync(User, testId, "raven") && !await _userManager.IsInRoleAsync(User, "Administrator"))
        {
            return Forbid();
        }

        RavenTest = _context.RavenTests.FirstOrDefault(ct => ct.Id == testId)!;

        if (string.IsNullOrEmpty(RavenTest.Response))
        {
            return RedirectToPage("TakeTest", new { testId });
        }

        Score = RavenTest.CalculateScores();

        if (RavenTest.Result == "Wait for AI" || string.IsNullOrEmpty(RavenTest.Result))
        {
            // Get result from AI
            var aiResponse = await _AIService.GetAIReplyForTestAsync(Score.ToJson(), "Raven PI-R");
            RavenTest.Result = aiResponse.IsSuccess ? aiResponse.Reply : "Wait for AI";
            await _context.SaveChangesAsync();
            if(!aiResponse.IsSuccess) AIError = aiResponse.Reply;
        }
        return Page();
    }
}