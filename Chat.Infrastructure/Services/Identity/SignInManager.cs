using System.Security.Claims;
using Chat.Application.Constants.Identity;
using Chat.Application.Entities.IdentityEntities;
using Chat.Application.Interfaces.Identity;
using Chat.Domain.Common.Results;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Chat.Infrastructure.Services.Identity;

public class SignInManager : ISignInManager
{
    private readonly IUserManager _userManager;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserClaimsPrincipalFactory _userClaimsPrincipalFactory;

    private readonly IHttpContextAccessor _contextAccessor;

    public IUserClaimsPrincipalFactory ClaimsFactory => _userClaimsPrincipalFactory;

    public HttpContext Context
    {
        get
        {
            var context = _contextAccessor?.HttpContext;
            
            if (context == null)
            {
                throw new InvalidOperationException("HttpContext must not be null.");
            }
            
            return context;
        }
    }

    public SignInManager(IPasswordHasher passwordHasher, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory userClaimsPrincipalFactory, IUserManager userManager)
    {
        _passwordHasher = passwordHasher;
        _contextAccessor = contextAccessor;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _userManager = userManager;
    }
    
    public bool IsSignedIn(ClaimsPrincipal principal)
    {
        return principal.Identities.Any(i => i.AuthenticationType == IdentityConstants.ApplicationScheme);
    }

    public async Task<Result> CanSignInAsync(User user)
    {
        var result = new Result();

        return result.Successful();
    }
    
    public Task SignInAsync(User user, AuthenticationProperties authenticationProperties, string? authenticationMethod = null)
    {
        IList<Claim> additionalClaims;
        
        if (authenticationMethod != null)
        {
            additionalClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.AuthenticationMethod, authenticationMethod),
            };
        }
        else
        {
            additionalClaims = Array.Empty<Claim>();
        }
        
        return SignInWithClaimsAsync(user, authenticationProperties, additionalClaims);
    }
    
    public async Task SignInWithClaimsAsync(User user, AuthenticationProperties? authenticationProperties, IEnumerable<Claim> additionalClaims)
    {
        var userPrincipal = await ClaimsFactory.CreateAsync(user);
        
        userPrincipal.Identities.First().AddClaims(additionalClaims);
        
        await Context.SignInAsync(IdentityConstants.ApplicationScheme, userPrincipal, authenticationProperties ?? new AuthenticationProperties());
    }
    
    public async Task SignOutAsync()
    {
        await Context.SignOutAsync(IdentityConstants.ApplicationScheme);
        await Context.SignOutAsync(IdentityConstants.ExternalScheme);
    }
    
    public async Task<Result> PasswordSignInAsync(User user, string password, AuthenticationProperties authenticationProperties)
    {
        var attempt = await CheckPasswordSignInAsync(user, password);
        
        if (!attempt.Succeeded)
            return attempt;
        
        await SignInWithClaimsAsync(user, authenticationProperties, new Claim[] { new Claim("amr", "pwd") });

        return attempt;
    }
    
    public async Task<Result> CheckPasswordSignInAsync(User user, string password)
    {
        var canSignInResult = await CanSignInAsync(user);
        
        if (!canSignInResult.Succeeded)
        {
            return canSignInResult;
        }

        var result = await _userManager.CheckPasswordAsync(user, password);

        if (!result.Succeeded)
        {
            return result.WithError("Wrong password was provided!", "wrongPassword");
        }

        return result.Successful();
    }
}