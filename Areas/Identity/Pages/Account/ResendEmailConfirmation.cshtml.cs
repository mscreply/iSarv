using iSarv.Data.CultureModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using iSarv.Data;
using iSarv.Data.Services;

namespace iSarv.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResendEmailConfirmationModel : PageModel
    {
        private readonly ApplicationUserManager _userManager;
        private readonly IEmailService _emailService;
        private readonly CultureLocalizer _localizer;

        public ResendEmailConfirmationModel(ApplicationUserManager userManager, IEmailService emailService,
            CultureLocalizer localizer)
        {
            _userManager = userManager;
            _emailService = emailService;
            _localizer = localizer;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress(ErrorMessage = "Please enter a valid email.")]
            [Display(Name = "Email", Prompt = "Email")]
            public string Email { get; set; }
        }

        public void OnGet(string email = null)
        {
            Input = new InputModel { Email = email };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, _localizer.Text("Verification email sent. Please check your email."));
                return Page();
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: Request.Scheme);
            await _emailService.SendEmailAsync(
                Input.Email,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            ModelState.AddModelError(string.Empty, _localizer.Text("Verification email sent. Please check your email."));
            return Page();
        }
    }
}
