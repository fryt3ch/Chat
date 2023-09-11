using System.Security.Claims;
using Chat.Application.Entities.Identity;
using Chat.Domain.Common.Results;

namespace Chat.Application.Interfaces.Identity;

public interface IUserManager
{
    public Task<Result> CreateAsync(User user);

    public Task<Result> DeleteAsync(User user);

    public Task<Result> CheckPasswordAsync(User user, string password);

    public Task<User?> FindByUsernameAsync(string username);

    public Task<IList<string>> GetRolesAsync(User user);

    public Task<IList<Claim>> GetClaimsAsync(User user);
}