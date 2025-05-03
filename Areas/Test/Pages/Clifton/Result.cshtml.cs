using iSarv.Data;
using iSarv.Data.Services;
using iSarv.Data.Tests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NuGet.Protocol;

namespace iSarv.Areas.Test.Pages.Clifton;
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

    public CliftonTest CliftonTest { get; set; } = default!;

    public Dictionary<CliftonDomain, Dictionary<CliftonTheme, int>> Score { get; set; }

    public string AIError { get; set; }

    public async Task<IActionResult> OnGetAsync(int testId)
    {
        if (!await _userManager.DoesTestBelongToUserAsync(User, testId, "clifton") && !await _userManager.IsInRoleAsync(User, "Administrator"))
        {
            return Forbid();
        }

        CliftonTest = _context.CliftonTests.FirstOrDefault(ct => ct.Id == testId)!;
        if (string.IsNullOrEmpty(CliftonTest.Response))
        {
            return RedirectToPage("TakeTest", new { testId });
        }

        Score = CliftonTest.CalculateScores();

        if (CliftonTest.Result == "Wait for AI" || string.IsNullOrEmpty(CliftonTest.Result))
        {
            // Get result from AI
            var aiResponse = await _AIService.GetAvalAIReplyForTestAsync(Score.ToJson(), "Clifton Strengths");
            CliftonTest.Result = aiResponse.IsSuccess ? aiResponse.Reply : "Wait for AI";
            await _context.SaveChangesAsync();
            if(!aiResponse.IsSuccess) AIError = aiResponse.Reply;
        }
        return Page();
    }
}