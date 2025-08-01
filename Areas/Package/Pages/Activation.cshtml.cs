using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using iSarv.Data;
using System.Security.Claims;
using iSarv.Data.Tests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace iSarv.Areas.Test.Pages;

[Authorize]
public class Activation : PageModel
{
    private readonly ApplicationDbContext _context;
    [TempData] public string ToastMessage { get; set; }
    [BindProperty] public string activationCode { get; set; }

    public Activation(ApplicationDbContext context)
    {
        _context = context;
    }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (!string.IsNullOrEmpty(activationCode))
        {
            // Check if the activation code exists in the database
            var activationCodeExists = _context.ActivationCodes.Any(a => a.Code == activationCode);

            if (activationCodeExists)
            {
                var activationCodeInfo = _context.ActivationCodes.FirstOrDefault(a => a.Code == activationCode);
                // Check if any other fields are not empty and they should be equal to User respective info
                // For example, check if Email is not empty
                // and it should be equal to the user's email
                var user = _context.Users.Find(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (!string.IsNullOrEmpty(activationCodeInfo.Email) && activationCodeInfo.Email != user.Email)
                {
                    ModelState.AddModelError("activationCode", "Invalid Email.");
                    ToastMessage = "This Code does not belong to you.";
                    return Page();
                }

                if (!string.IsNullOrEmpty(activationCodeInfo.PhoneNumber) && activationCodeInfo.PhoneNumber != user.PhoneNumber)
                {
                    ModelState.AddModelError("activationCode", "Invalid Phone Number.");
                    ToastMessage = "This Code does not belong to you.";
                    return Page();
                }

                if (!string.IsNullOrEmpty(activationCodeInfo.NationalId) &&
                    activationCodeInfo.NationalId != user.NationalId)
                {
                    ModelState.AddModelError("activationCode", "Invalid National Id.");
                    ToastMessage = "This Code does not belong to you.";
                    return Page();
                }

                // If all checks pass, proceed with activation
                // Activation code exists, perform activation logic
                // For example, set a flag in the database to indicate that the package is activated
                var testPackage = new TestPackage()
                {
                    UserId = user.Id
                };
                _context.TestPackages.Add(testPackage);
                _context.ActivationCodes.Remove(activationCodeInfo);
                _context.SaveChanges();
                ToastMessage = "Activation code is valid";
                return RedirectToPage("Dashboard", new { area = "User" });
            }
            else
            {
                // Activation code does not exist, display an error message
                ModelState.AddModelError("activationCode", "Invalid activation code");
                ToastMessage = "Invalid activation code";
                return Page();
            }
        }
        else
        {
            // Activation code is empty, display an error message
            ModelState.AddModelError("activationCode", "Activation code is required");
            ToastMessage = "Activation code is required";
            return Page();
        }
    }
}