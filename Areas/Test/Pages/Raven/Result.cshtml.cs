using iSarv.Data;
using iSarv.Data.Services;
using iSarv.Data.Tests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace iSarv.Areas.Test.Pages.Raven;

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

    public RavenTest RavenTest { get; set; } = default!;

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

        return Page();
    }
}