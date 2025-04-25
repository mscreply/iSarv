using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace iSarv.Data;

public class ApplicationUserManager: UserManager<ApplicationUser>
{
    public ApplicationUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators, IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<ApplicationUserManager> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
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

    public async Task<string> GetFullnameAsync(ClaimsPrincipal claimsPrincipal)
    {
        var user = await GetUserAsync(claimsPrincipal);
        return user!.FullName;
    }
}