using iSarv.Data;
using iSarv.Data.Services;
using iSarv.Data.Tests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace iSarv.Areas.Test.Pages.Neo;

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

    public NeoTest NeoTest { get; set; } = default!;

    public Dictionary<NeoPersonalityInventory, Dictionary<NeoFacets, int>> Score { get; set; }

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

        return Page();
    }
}