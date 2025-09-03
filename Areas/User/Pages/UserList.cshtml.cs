using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using iSarv.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace iSarv.Areas.User.Pages
{
    [Authorize(Roles = "Administrator")]
    public class UserListModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserListModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<UserViewModel> Users { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }

        [BindProperty(SupportsGet = true)] public string SearchTerm { get; set; }

        [TempData] public string ToastMessage { get; set; }

        public async Task OnGetAsync(int currentPage = 1, string searchTerm = "")
        {
            CurrentPage = currentPage;
            SearchTerm = searchTerm;
            var adminUsers = (await _userManager.GetUsersInRoleAsync("Admin")).ToList();

            var allUsers = _context.Users.Where(u => !adminUsers.Contains(u)).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                allUsers = allUsers.Where(u => u.FullName.Contains(searchTerm) || u.Email.Contains(searchTerm) ||
                                               u.PhoneNumber.Contains(searchTerm) || u.NationalId.Contains(searchTerm));
            }

            TotalPages = (int)Math.Ceiling(allUsers.Count() / (double)PageSize);

            var users = allUsers
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize);

            Users = new List<UserViewModel>();
            foreach (var user in users)
            {
                Users.Add(new UserViewModel
                {
                    Email = user.Email,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    Id = user.Id,
                    NationalId = user.NationalId,
                    University = user.University,
                    IsAdmin = await _userManager.IsInRoleAsync(user, "Administrator"),
                    IsPsychologist = await _userManager.IsInRoleAsync(user, "Psychologist"),
                    IsActive = user.IsActive,
                    EmailConfirmed = user.EmailConfirmed,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed
                });
            }
        }

        public class UserViewModel
        {
            public string Id { get; set; }
            public string Email { get; set; }
            public string FullName { get; set; }
            public string PhoneNumber { get; set; }
            public string NationalId { get; set; }
            public string University { get; set; }
            public bool IsAdmin { get; set; }
            public bool IsActive { get; set; }
            public bool EmailConfirmed { get; set; }
            public bool PhoneNumberConfirmed { get; set; }
            public bool IsPsychologist { get; set; }
        }

        public async Task<IActionResult> OnPostAssignAdminAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, "Administrator");

            if (isAdmin)
            {
                await _userManager.RemoveFromRoleAsync(user, "Administrator");
                ToastMessage = $"User {user.Email} removed from admin role.";
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "Administrator");
                ToastMessage = $"User {user.Email} added to admin role.";
            }

            return RedirectToPage("./UserList", new { area = "User", currentPage = CurrentPage });
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            ToastMessage = $"User deleted successfully.";
            return RedirectToPage("./UserList", new { area = "User", currentPage = CurrentPage });
        }

        public async Task<IActionResult> OnPostResetPasswordAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var resetPasswordResult = await _userManager.ResetPasswordAsync(user,
                await _userManager.GeneratePasswordResetTokenAsync(user), "123456");
            if (!resetPasswordResult.Succeeded)
            {
                foreach (var error in resetPasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return RedirectToPage("./UserList", new { currentPage = CurrentPage });
            }

            ToastMessage = "Password reset successfully.";
            return RedirectToPage("./UserList", new { area = "User", currentPage = CurrentPage });
        }

        public async Task<IActionResult> OnPostToggleActiveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            user.IsActive = !user.IsActive;
            await _userManager.UpdateAsync(user);

            ToastMessage = $"User is now {(user.IsActive ? "active" : "inactive")}.";
            return RedirectToPage("./UserList", new { area = "User", currentPage = CurrentPage });
        }

        public async Task<IActionResult> OnPostToggleEmailConfirmedAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            user.EmailConfirmed = !user.EmailConfirmed;
            await _userManager.UpdateAsync(user);

            ToastMessage = $"Email confirmation toggled successfully.";
            return RedirectToPage("./UserList", new { area = "User", currentPage = CurrentPage });
        }

        public async Task<IActionResult> OnPostTogglePhoneNumberConfirmedAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            user.PhoneNumberConfirmed = !user.PhoneNumberConfirmed;
            await _userManager.UpdateAsync(user);

            ToastMessage = $"Phone number confirmation toggled successfully.";
            return RedirectToPage("./UserList", new { area = "User", currentPage = CurrentPage });
        }

        public async Task<IActionResult> OnPostTogglePsychologistAsync(string id, int currentPage)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ToastMessage"] = "User not found.";
                return RedirectToPage(new { currentPage });
            }

            var isPsychologist = await _userManager.IsInRoleAsync(user, "Psychologist");
            var result = isPsychologist
                ? await _userManager.RemoveFromRoleAsync(user, "Psychologist")
                : await _userManager.AddToRoleAsync(user, "Psychologist");

            if (result.Succeeded)
            {
                TempData["ToastMessage"] = "Psychologist status updated successfully.";
            }
            else
            {
                TempData["ToastMessage"] = "Failed to update psychologist status.";
            }

            return RedirectToPage(new { currentPage });
        }
        
        public async Task<IActionResult> OnPostLoadFromExcelAsync(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                ToastMessage = "Please upload a valid Excel file.";
                return RedirectToPage();
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    await excelFile.CopyToAsync(stream);
                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                        {
                            ToastMessage = "The uploaded Excel file contains no worksheets.";
                            return RedirectToPage();
                        }

                        var rowCount = worksheet.RowsUsed().Count();
                        if (rowCount < 2) // Assuming the first row is the header
                        {
                            ToastMessage = "The uploaded Excel file is empty or has no data rows.";
                            return RedirectToPage();
                        }

                        foreach (var row in worksheet.RowsUsed().Skip(1)) // Skip header row
                        {
                            var userName = row.Cell(1).GetString();
                            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
                            if (user == null)
                            {
                                user = new ApplicationUser()
                                {
                                    UserName = row.Cell(1).GetString(),
                                    Email = row.Cell(2).GetString() ?? "",
                                    PhoneNumber = row.Cell(1).GetString(),
                                    PhoneNumberConfirmed = true,
                                    FullName = row.Cell(3).GetString() ?? "",
                                    NationalId = row.Cell(4).GetString() ?? "",
                                    DateOfBirth = DateTime.TryParse(row.Cell(5).GetString(), out var date) ? date : DateTime.Now,
                                    Gender = Enum.TryParse<Gender>(row.Cell(6).GetString(), out var gender) ? gender : default,
                                    Address = row.Cell(7).GetString() ?? "",
                                    Bio = row.Cell(8).GetString() ?? "",
                                    Occupation = row.Cell(9).GetString() ?? "",
                                    FieldOfStudy = row.Cell(10).GetString() ?? "",
                                    University = row.Cell(11).GetString() ?? ""
                                };
                                await _userManager.CreateAsync(user, "123456");
                            }

                            await _context.SaveChangesAsync();
                        }
                    }
                }

                ToastMessage = "Users loaded successfully from Excel.";
            }
            catch (Exception ex)
            {
                ToastMessage = $"An error occurred while processing the file: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}