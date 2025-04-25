using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using iSarv.Data;

namespace iSarv.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class PhoneNumberConfirmationModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationUserManager _userManager;
        private ISmsService _smsService;

        public PhoneNumberConfirmationModel(SignInManager<ApplicationUser> signInManager, ApplicationUserManager userManager, IEmailService Service, ISmsService smsService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _smsService = smsService;
        }

        public string PhoneNumber { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "{0} is required.")]
            [RegularExpression("([0-9]{6})", ErrorMessage = "Please enter the 6-digit number sent.")]
            [Display(Name = "Token", Prompt = "Token")]
            public string Token { get; set; }

            [Required(ErrorMessage = "{0} is required.")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password", Prompt = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm Password", Prompt = "Confirm Password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        [BindProperty] public InputModel Input { get; set; }

        public IActionResult OnGet(string phoneNumber, string returnUrl = null)
        {
            if (phoneNumber == null)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                PhoneNumber = phoneNumber;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string phoneNumber, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = phoneNumber != null ? await _userManager.FindByNameAsync(phoneNumber) : null;
                if (user == null)
                    return RedirectToPage("PhoneRegister", new { returnUrl = returnUrl });
                var result = await _userManager.ConfirmPhoneNumberAsync(user, Input.Token);
                if (result)
                {
                    await _userManager.RemovePasswordAsync(user);
                    await _userManager.AddPasswordAsync(user, Input.Password);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect("/Identity/Account/Manage/");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error");
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostResendSmsConfirmationAsync(string phoneNumber, string returnUrl)
        {
            returnUrl ??= "~/";
            var user = await _userManager.FindByNameAsync(phoneNumber);
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
            _smsService.SendSms($"کد تایید شما در آی‌سرو: {code}", user.PhoneNumber);
            // send the code by SMS
            Console.WriteLine(code);
            return RedirectToPage("PhoneNumberConfirmation",
                new { phoneNumber = phoneNumber, returnUrl = returnUrl });
        }
    }
}