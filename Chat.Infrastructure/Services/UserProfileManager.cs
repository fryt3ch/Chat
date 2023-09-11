using Chat.Application.Entities;
using Chat.Application.Interfaces;
using Chat.Application.Interfaces.Identity;
using Chat.Domain.Common.Results;
using Chat.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Chat.Infrastructure.Services;

public class UserProfileManager : IUserProfileManager
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILookupNormalizer _lookupNormalizer;

    public UserProfileManager(ApplicationDbContext dbContext, ILookupNormalizer lookupNormalizer)
    {
        _dbContext = dbContext;
        _lookupNormalizer = lookupNormalizer;
    }

    public async Task<Result> CreateAsync(UserProfile userProfile)
    {
        var result = new Result();
        
        await _dbContext.UserProfiles.AddAsync(userProfile);

        await _dbContext.SaveChangesAsync();

        return result.Successful();
    }

    public async Task<UserProfile?> GetByIdAsync(Guid id)
    {
        return await _dbContext.UserProfiles.FindAsync(id);
    }

    public Task<UserProfile?> GetByUserIdAsync(Guid userId)
    {
        return GetByIdAsync(userId);
    }

    public async Task<UserProfile?> GetByUsernameAsync(string username)
    {
        username = _lookupNormalizer.NormalizeName(username);
        
        var userId = await _dbContext.Users.Where(x => x.NormalizedUsername == username).Select(x => x.Id).FirstOrDefaultAsync();

        if (userId == Guid.Empty)
            return null;

        return await GetByUserIdAsync(userId);
    }

    public async Task<string> GetUsernameAsync(UserProfile userProfile)
    {
        return (await _dbContext.Users.Where(x => x.Id == userProfile.Id).Select(x => x.Username).FirstOrDefaultAsync())!;
    }
}