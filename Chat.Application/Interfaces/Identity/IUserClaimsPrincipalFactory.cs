using System.Security.Claims;
using Chat.Application.Entities.IdentityEntities;

namespace Chat.Application.Interfaces.Identity;

public interface IUserClaimsPrincipalFactory
{
    public Task<ClaimsPrincipal> CreateAsync(User user);
}