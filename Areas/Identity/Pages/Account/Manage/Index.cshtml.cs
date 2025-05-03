// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using iSarv.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using iSarv.Data;

namespace iSarv.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationUserManager _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            ApplicationUserManager userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData] public string StatusMessage { get; set; }

        [BindProperty] public ApplicationUser ApplicationUser { get; set; } = new();
        public List<(string English, string Farsi)> FieldOfStudyOptions { get; set; } = new();
        public List<(string English, string Farsi)> OccupationOptions { get; set; } = new();
        public List<(string English, string Farsi)> UniversityOptions { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            ApplicationUser = await _userManager.GetUserAsync(User);
            if (ApplicationUser == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
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
                StatusMessage = "Error reading Fields: " + e.Message;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            user.FullName = ApplicationUser.FullName;
            user.NationalId = ApplicationUser.NationalId;
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

            return Redirect("/User/Dashboard");
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
}
