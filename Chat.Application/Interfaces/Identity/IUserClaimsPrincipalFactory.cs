using System.Security.Claims;
using Chat.Application.Entities.Identity;

namespace Chat.Application.Interfaces.Identity;

public interface IUserClaimsPrincipalFactory
{
    public Task<ClaimsPrincipal> CreateAsync(User user);
}