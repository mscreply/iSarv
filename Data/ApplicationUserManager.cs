using System.Security.Claims;
using iSarv.Data.Tests;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace iSarv.Data;

public class ApplicationUserManager : UserManager<ApplicationUser>
{
    public ApplicationUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators,
        IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors, IServiceProvider services, ILogger<ApplicationUserManager> logger) : base(store,
        optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
    }

    public async Task<ApplicationUser> FindByPhoneNumberAsync(string phoneNumber)
    {
        return (await Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber))!;
    }

    public async Task<bool> IsConfirmedAsync(ApplicationUser user)
    {
        return await IsEmailConfirmedAsync(user) || await IsPhoneNumberConfirmedAsync(user);
    }

    public async Task<bool> ConfirmPhoneNumberAsync(ApplicationUser user, string token)
    {
        if (await VerifyChangePhoneNumberTokenAsync(user, token, user.PhoneNumber!))
        {
            user.PhoneNumberConfirmed = true;
            await UpdateAsync(user);
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<string> GetFullnameAsync(ClaimsPrincipal claimsPrincipal, bool includeNationalId = false)
    {
        var user = await GetUserAsync(claimsPrincipal);
        return includeNationalId ? $"{user!.FullName} ({user.NationalId})" : user!.FullName;
    }   

    public async Task<bool> IsInRoleAsync(ClaimsPrincipal claimsPrincipal, string roleName)
    {
        var user = await GetUserAsync(claimsPrincipal);
        return user != null && await IsInRoleAsync(user, roleName);
    }

    public async Task<List<TestPackage>> GetUserPackagesAsync(ClaimsPrincipal claimsPrincipal)
    {
        var user = await GetUserAsync(claimsPrincipal);
        return await Users
            .Where(u => u.Id == user!.Id)
            .SelectMany(u => u.TestPackages)
            .Include(tp => tp.CliftonTest)
            .Include(tp => tp.RavensTest)
            .Include(tp => tp.HollandsTest)
            .Include(tp => tp.NeoTest)
            .ToListAsync();
    }

    public async Task<ApplicationUser> GetUserIncludeTestPackagesAsync(ClaimsPrincipal claimsPrincipal)
    {
        var user = await GetUserAsync(claimsPrincipal);
        return (await Users
            .Where(u => u.Id == user!.Id)
            .Include(u => u.TestPackages)
            .ThenInclude(tp => tp.CliftonTest)
            .Include(u => u.TestPackages)
            .ThenInclude(tp => tp.RavensTest)
            .Include(u => u.TestPackages)
            .ThenInclude(tp => tp.HollandsTest)
            .Include(u => u.TestPackages)
            .ThenInclude(tp => tp.NeoTest)
            .FirstOrDefaultAsync())!;
    }

    public async Task<bool> DoesTestBelongToUserAsync(ClaimsPrincipal claimsPrincipal, int testId, string testType)
    {
        var userPackages = await GetUserPackagesAsync(claimsPrincipal);
        return userPackages.Any(tp =>
        {
            switch (testType.ToLower())
            {
                case "clifton":
                    return tp.CliftonTest != null && tp.CliftonTest.Id == testId;
                case "ravens":
                    return tp.RavensTest != null && tp.RavensTest.Id == testId;
                case "hollands":
                    return tp.HollandsTest != null && tp.HollandsTest.Id == testId;
                case "neo":
                    return tp.NeoTest != null && tp.NeoTest.Id == testId;
                default:
                    return false;
            }
        });
    }

    public async Task<bool>
        IsInfoCompletedAsync(ClaimsPrincipal claimsPrincipal) // Changed to async and added ClaimsPrincipal parameter
    {
        var user = await GetUserAsync(claimsPrincipal); // Get the ApplicationUser from ClaimsPrincipal

        if (user == null)
        {
            return false; // User not found
        }

        // Check if required fields are completed
        return !string.IsNullOrEmpty(user.FullName) &&
               !string.IsNullOrEmpty(user.Email) &&
               !string.IsNullOrEmpty(user.PhoneNumber) &&
               !string.IsNullOrEmpty(user.NationalId) &&
               user.DateOfBirth < DateTime.Now && // Check if DateOfBirth has been set
               !string.IsNullOrEmpty(user.Address) &&
               !string.IsNullOrEmpty(user.Bio) &&
               !string.IsNullOrEmpty(user.Occupation) &&
               !string.IsNullOrEmpty(user.FieldOfStudy) &&
               !string.IsNullOrEmpty(user.University);
    }
}