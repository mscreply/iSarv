using iSarv.Data;
using iSarv.Data.Services;
using iSarv.Data.Tests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace iSarv.Areas.Test.Pages.Holland;

[Authorize]
public class Result : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly ApplicationUserManager _userManager;

    public Result(ApplicationDbContext context, ApplicationUserManager userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public HollandTest HollandTest { get; set; } = default!;

    public Dictionary<HollandPersonality, int> Score { get; set; } = default!;
    public async Task<IActionResult> OnGetAsync(int testId)
    {
        if (!await _userManager.DoesTestBelongToUserAsync(User, testId, "holland") && !await _userManager.IsInRoleAsync(User, "Administrator"))
        {
            return Forbid();
        }

        HollandTest = _context.HollandTests.FirstOrDefault(ct => ct.Id == testId)!;

        if (string.IsNullOrEmpty(HollandTest.Response))
        {
            return RedirectToPage("TakeTest", new { testId });
        }


        Score = HollandTest.CalculateScores();

        return Page();
    }
}
