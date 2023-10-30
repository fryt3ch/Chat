using System.Security.Claims;
using Chat.Application.Entities.IdentityEntities;
using Chat.Domain.Common.Results;
using Microsoft.AspNetCore.Authentication;

namespace Chat.Application.Interfaces.Identity;

public interface ISignInManager
{
    public bool IsSignedIn(ClaimsPrincipal principal);
    
    public Task<Result> CanSignInAsync(User user);

    public Task SignInAsync(User user, AuthenticationProperties authenticationProperties,
        string? authenticationMethod = null);

    public Task SignInWithClaimsAsync(User user, AuthenticationProperties? authenticationProperties,
        IEnumerable<Claim> additionalClaims);
    
    public Task SignOutAsync();

    public Task<Result> PasswordSignInAsync(User user, string password,
        AuthenticationProperties authenticationProperties);

    public Task<Result> CheckPasswordSignInAsync(User user, string password);
}