using iSarv.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace iSarv.Areas.User.Pages;

[Authorize]
public class Dashboard : PageModel
{
    private readonly ApplicationUserManager _userManager;

    public Dashboard(ApplicationUserManager userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if(!await _userManager.IsInfoCompletedAsync(User))
            return Redirect("/Identity/Account/Manage/");
        return Page();
    }
}