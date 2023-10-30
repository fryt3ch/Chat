using Chat.Application.Dto.UserProfile;
using Chat.Application.Entities;
using Chat.Application.Entities.UserProfileEntities;
using Chat.Domain.Common.Results;

namespace Chat.Application.Interfaces;

public interface IUserProfileManager
{
    public Task<Result> CreateAsync(UserProfile userProfile);
    
    public Task<UserProfile?> GetByIdAsync(Guid id);
    public Task<UserProfile?> GetByUserIdAsync(Guid userId);
    public Task<UserProfile?> GetByUsernameAsync(string username);
    
    public Task<Result<string>> PostPhotoAsync(UserProfile userProfile, byte[] imageBytes);
    public Task<Result<byte[]>> GetPhotoAsync(string userId, string photoId);
    public Task<Result> UpdateAvatarAsync(UserProfile userProfile, string photoId);
    public Task<Result> DeleteAvatarAsync(UserProfile userProfile, bool deleteImage);
    
    public Task<Result<UserProfileDto>> GetUserProfileAsync(string idOrUsername, UserProfileRequestDto dto);
}