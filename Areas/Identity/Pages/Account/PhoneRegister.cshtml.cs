using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using iSarv.Data;
using iSarv.Data.Services;

namespace iSarv.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class PhoneRegisterModel : PageModel
    {
        private readonly ApplicationUserManager _userManager;
        private readonly ILogger<PhoneRegisterModel> _logger;
        private readonly ISmsService _smsService;

        public PhoneRegisterModel(
            ApplicationUserManager userManager,
            ILogger<PhoneRegisterModel> logger, ISmsService smsService)
        {
            _userManager = userManager;
            _logger = logger;
            _smsService = smsService;
        }

        [TempData] public string ToastMessage { get; set; }
        [BindProperty] public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "{0} is required.")]
            [RegularExpression(@"0\d{10}", ErrorMessage = "Please enter a valid phone number.")]
            [Display(Name = "Phone Number", Prompt = "Phone Number")]
            public string PhoneNumber { get; set; }
        }

        public Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            return Task.CompletedTask;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(Input.PhoneNumber);
                if (user == null)
                {
                    user = new ApplicationUser { PhoneNumber = Input.PhoneNumber, PhoneNumberConfirmed = false, UserName = Input.PhoneNumber };
                    await _userManager.CreateAsync(user);
                }
                else
                {
                    user.PhoneNumberConfirmed = false;
                }

                _logger.LogInformation("User created a new account with password");

                var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
                // send the code by SMS
                var response = _smsService.SendSms($"کد تایید شما در آی‌سرو: {code}", Input.PhoneNumber);
                if (response.IsSuccess)
                {
                    return RedirectToPage("PhoneNumberConfirmation",
                        new { phoneNumber = Input.PhoneNumber, returnUrl = returnUrl });
                }
                else
                {
                    ToastMessage = response.Result;
                    return RedirectToPage();
                }
                // If we got this far, something failed, redisplay form
            }

            return Page();
        }
    }
}