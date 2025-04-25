using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using iSarv.Data;
using Microsoft.AspNetCore.Authorization;
using RIMS.Data;

namespace iSarv.Areas.User.Pages;

[Authorize]
public class EditApplicationUserModel : PageModel
{
    private readonly ApplicationUserManager _userManager;

    public EditApplicationUserModel(ApplicationUserManager userManager)
    {
        _userManager = userManager;
    }


    [TempData] public string StatusMessage { get; set; }

    [BindProperty] public ApplicationUser ApplicationUser { get; set; } = new();
    public string Username { get; set; }
    public List<(string English, string Farsi)> FieldOfStudyOptions { get; set; } = new();
    public List<(string English, string Farsi)> OccupationOptions { get; set; } = new();
    public List<(string English, string Farsi)> UniversityOptions { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(string id)
    {
        ApplicationUser = await _userManager.FindByIdAsync(id);
        if (ApplicationUser == null)
        {
            return NotFound();
        }

        Username = ApplicationUser.UserName;

        try
        {
            FieldOfStudyOptions = Utilities.ReadTextFileWithTranslations("Data/FieldOfStudy.txt");
            OccupationOptions = Utilities.ReadTextFileWithTranslations("Data/Occupation.txt");
            UniversityOptions = Utilities.ReadTextFileWithTranslations("Data/University.txt");
        }
        catch (Exception e)
        {
            // Handle the exception, e.g., log it or display an error message
            StatusMessage = "Error reading Field of Study options: " + e.Message;
        }

        var userId = _userManager.GetUserId(User);
        var isAdmin = User.IsInRole("Admin");

        if (userId != id && !isAdmin)
        {
            return RedirectToPage("/Error"); // Redirect to error page
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return RedirectToPage();
        }

        var user = await _userManager.FindByIdAsync(ApplicationUser.Id);
        if (user == null)
        {
            return NotFound();
        }

        user.FullName = ApplicationUser.FullName;
        user.DateOfBirth = ApplicationUser.DateOfBirth;
        user.Gender = ApplicationUser.Gender;
        user.Address = ApplicationUser.Address;
        user.Bio = ApplicationUser.Bio;
        user.Occupation = ApplicationUser.Occupation;
        user.FieldOfStudy = ApplicationUser.FieldOfStudy;
        user.University = ApplicationUser.University;

        if (user.UserName.Contains('@'))
        {
            if (user.PhoneNumber != ApplicationUser.PhoneNumber)
            {
                user.PhoneNumberConfirmed = false;
                await _userManager.SetPhoneNumberAsync(user, ApplicationUser.PhoneNumber);
            }
        }
        else
        {
            if (user.Email != ApplicationUser.Email)
            {
                user.EmailConfirmed = false;
                await _userManager.SetEmailAsync(user, ApplicationUser.Email);
            }
        }

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return RedirectToPage();
        }

        return RedirectToPage("./UserList");
    }

    public async Task<IActionResult> OnPostAddPhotoAsync(string id, IFormFile file)
    {
        try
        {
            await Utilities.SaveToFileAsync($"img/avatars/{id}.png", file);
            StatusMessage = SuccessMessages.OperationSuccessful;
        }
        catch
        {
            StatusMessage = ErrorMessages.OperationFailed;
        }

        return RedirectToPage();
    }
}
