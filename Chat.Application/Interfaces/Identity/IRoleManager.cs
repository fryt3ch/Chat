using System.Security.Claims;
using Chat.Application.Entities.Identity;
using Chat.Domain.Common.Results;

namespace Chat.Application.Interfaces.Identity;

public interface IRoleManager
{
    public Task<Result> CreateAsync(Role role);
    
    public Task<Result> UpdateAsync(Role role);

    public Task<Result> DeleteAsync (Role role);

    public Task<IList<Claim>> GetClaimsAsync(Role role);

    public Task<Result> AddClaimAsync(Role role, Claim claim);

    public Task<Role?> FindByNameAsync(string name);
}