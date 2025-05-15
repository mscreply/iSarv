using iSarv.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using iSarv.Data;
using Microsoft.AspNetCore.Identity;

namespace iSarv.Areas.User.Pages
{
    [Authorize(Roles = "Administrator")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;


        public CreateModel(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            try
            {
                OccupationOptions = Utilities.ReadTextFileWithTranslations("App_Files/data/Occupation.txt");
                FieldOfStudyOptions = Utilities.ReadTextFileWithTranslations("App_Files/data/FieldOfStudy.txt");
                UniversityOptions = Utilities.ReadTextFileWithTranslations("App_Files/data/University.txt");
                AvailableRoles = _roleManager.Roles.Where(r => r.Name != "Admin").Select(r => r.Name).ToList();
            }
            catch (Exception e)
            {
                // Handle the exception, e.g., log it or display an error message
                StatusMessage = "Error reading options: " + e.Message;
            }
            return Page();
        }

        [BindProperty]
        public ApplicationUser ApplicationUser { get; set; } = default!;

        [BindProperty]
        public List<string> SelectedRoles { get; set; } = new List<string>();

        public List<(string English, string Farsi)> OccupationOptions { get; set; } = new();
        public List<(string English, string Farsi)> FieldOfStudyOptions { get; set; } = new();
        public List<(string English, string Farsi)> UniversityOptions { get; set; } = new();

        public List<string> AvailableRoles { get; set; } = new List<string>();

        [TempData] public string StatusMessage { get; set; }


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                try
                {
                    OccupationOptions = Utilities.ReadTextFileWithTranslations("App_Files/data/Occupation.txt");
                    FieldOfStudyOptions = Utilities.ReadTextFileWithTranslations("App_Files/data/FieldOfStudy.txt");
                    UniversityOptions = Utilities.ReadTextFileWithTranslations("App_Files/data/University.txt");
                    AvailableRoles = _roleManager.Roles.Where(r => r.Name != "Admin").Select(r => r.Name).ToList();
                }
                catch (Exception e)
                {
                    // Handle the exception, e.g., log it or display an error message
                    StatusMessage = "Error reading options: " + e.Message;
                }
                return Page();
            }

            var user = new ApplicationUser
            {
                UserName = ApplicationUser.PhoneNumber,
                Email = ApplicationUser.Email,
                FullName = ApplicationUser.FullName,
                NationalId = ApplicationUser.NationalId,
                DateOfBirth = ApplicationUser.DateOfBirth,
                Gender = ApplicationUser.Gender,
                Address = ApplicationUser.Address,
                Bio = ApplicationUser.Bio,
                Occupation = ApplicationUser.Occupation,
                FieldOfStudy = ApplicationUser.FieldOfStudy,
                University = ApplicationUser.University,
                PhoneNumber = ApplicationUser.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, "123456");

            if (result.Succeeded)
            {
                await _userManager.AddToRolesAsync(user, SelectedRoles);
                return RedirectToPage("./userList");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}
