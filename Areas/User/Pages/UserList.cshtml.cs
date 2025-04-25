using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using iSarv.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace iSarv.Areas.User.Pages
{
    [Authorize]
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

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [TempData] public string ToastMessage { get; set; }

        public async Task OnGetAsync(int currentPage = 1, string searchTerm = "")
        {
            CurrentPage = currentPage;
            SearchTerm = searchTerm;
            var adminUsers = (await _userManager.GetUsersInRoleAsync("Admin")).ToList();

            var allUsers = _context.Users.Where(u => !adminUsers.Contains(u)).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                allUsers = allUsers.Where(u => u.FullName.Contains(searchTerm) || u.Email.Contains(searchTerm) || u.PhoneNumber.Contains(searchTerm));
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
                    IsAdmin = await _userManager.IsInRoleAsync(user, "Administrator"),
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
            public bool IsAdmin { get; set; }
            public bool IsActive { get; set; }
            public bool EmailConfirmed { get; set; }
            public bool PhoneNumberConfirmed { get; set; }
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
    }
}
