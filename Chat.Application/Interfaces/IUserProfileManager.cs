using Chat.Application.Entities;
using Chat.Domain.Common.Results;

namespace Chat.Application.Interfaces;

public interface IUserProfileManager
{
    public Task<Result> CreateAsync(UserProfile userProfile);
    
    public Task<UserProfile?> GetByIdAsync(Guid id);
    public Task<UserProfile?> GetByUserIdAsync(Guid userId);
    public Task<UserProfile?> GetByUsernameAsync(string username);

    public Task<string> GetUsernameAsync(UserProfile userProfile);
}