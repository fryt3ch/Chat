using System.Security.Claims;
using Chat.Application.Constants.Identity;
using Chat.Application.Entities.IdentityEntities;
using Chat.Application.Interfaces.Identity;

namespace Chat.Infrastructure.Services.Identity;

public class UserClaimsPrincipalFactory : IUserClaimsPrincipalFactory
{
    private readonly IUserManager _userManager;
    private readonly IRoleManager _roleManager;

    public UserClaimsPrincipalFactory(IUserManager userManager, IRoleManager roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<ClaimsPrincipal> CreateAsync(User user)
    {
        var identity = await GenerateClaimsIdentityAsync(user);

        return new ClaimsPrincipal(identity);
    }
    
    protected async Task<ClaimsIdentity> GenerateClaimsIdentityAsync(User user)
    {
        var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme,
            IdentityConstants.UsernameClaimType,
            IdentityConstants.RoleClaimType);
        
        identity.AddClaim(new Claim(IdentityConstants.UserIdClaimType, user.Id.ToString()));
        identity.AddClaim(new Claim(IdentityConstants.UsernameClaimType, user.Username));

        var email = user.Email;
        
        if (!string.IsNullOrEmpty(email))
        {
            identity.AddClaim(new Claim(IdentityConstants.EmailClaimType, email));
        }
        
        //identity.AddClaims(await _userManager.GetClaimsAsync(user));

        foreach (var x in await _userManager.GetRolesAsync(user))
        {
            identity.AddClaim(new Claim(IdentityConstants.RoleClaimType, x));
            
            var role = await _roleManager.FindByNameAsync(x);

            if (role != null)
            {
                identity.AddClaims(await _roleManager.GetClaimsAsync(role));
            }
        }
        
        return identity;
    }
}