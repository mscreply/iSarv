using iSarv.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace iSarv.Data
{
    public static class InitSeedData
    {
        public static async Task CreateAdminAccountAsync(IServiceProvider
            serviceProvider, IConfiguration configuration)
        {
            serviceProvider = serviceProvider.CreateScope().ServiceProvider;
            UserManager<ApplicationUser> userManager =
                serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            RoleManager<IdentityRole> roleManager =
                serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string username = configuration["AdminUser:UserName"] ?? "09377899377";
            string email = configuration["AdminUser:Email"] ?? "mscreply@gmail.com";
            string phone = configuration["AdminUser:Phone"] ?? "09377899377";
            string password = configuration["AdminUser:Password"] ?? "123456";

            string[] roles = {"Admin", "Administrator"};
            foreach (var role in roles)
            {
                if (await roleManager.FindByNameAsync(role) == null)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            if (await userManager.FindByNameAsync(username) == null)
            {
                var user = new ApplicationUser
                {
                    UserName = username,
                    Email = email,
                    PhoneNumber = phone,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                };

                var result = await userManager
                    .CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                    await userManager.AddToRoleAsync(user, "Administrator");
                }
            }
        }
    }
}