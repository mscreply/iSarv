using iSarv.Data;
using iSarv.Data.Services;
using iSarv.Data.Tests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace iSarv.Areas.Test.Pages.Clifton;

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

    public CliftonTest CliftonTest { get; set; } = default!;

    public Dictionary<CliftonDomain, Dictionary<CliftonTheme, double>> Score { get; set; }

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

        return Page();
    }
}