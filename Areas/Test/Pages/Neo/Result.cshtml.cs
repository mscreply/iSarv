using iSarv.Data;
using iSarv.Data.Services;
using iSarv.Data.Tests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NuGet.Protocol;

namespace iSarv.Areas.Test.Pages.Neo;
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

    public NeoTest NeoTest { get; set; } = default!;

    public Dictionary<NeoPersonalityInventory, Dictionary<NeoFacets, int>> Score { get; set; }

    public string AIError { get; set; } = "";
    public async Task<IActionResult> OnGetAsync(int testId)
    {
        if (!await _userManager.DoesTestBelongToUserAsync(User, testId, "neo") && !await _userManager.IsInRoleAsync(User, "Administrator"))
        {
            return Forbid();
        }

        NeoTest = _context.NeoTests.FirstOrDefault(ct => ct.Id == testId)!;

        if (string.IsNullOrEmpty(NeoTest.Response))
        {
            return RedirectToPage("TakeTest", new { testId });
        }

        Score = NeoTest.CalculateScores();

        if (NeoTest.Result == "Wait for AI" || string.IsNullOrEmpty(NeoTest.Result))
        {
            // Get result from AI
            var aiResponse = await _AIService.GetAvalAIReplyForTestAsync(Score.ToJson(), "Neo PI-R");
            NeoTest.Result = aiResponse.IsSuccess ? aiResponse.Reply : "Wait for AI";
            await _context.SaveChangesAsync();
            if(!aiResponse.IsSuccess) AIError = aiResponse.Reply;
        }
        return Page();
    }
}