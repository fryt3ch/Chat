﻿using Chat.Application.Dto.UserProfile;
using Chat.Application.Entities;
using Chat.Application.Entities.IdentityEntities;
using Chat.Application.Entities.UserProfileEntities;
using Chat.Application.Interfaces;
using Chat.Application.Interfaces.Identity;
using Chat.Domain.Common.Results;
using Chat.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using FileNotFoundException = System.IO.FileNotFoundException;

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

    public async Task<Result<UserProfileDto>> GetUserProfileAsync(string idOrUsername, UserProfileRequestDto dto)
    {
        var result = new Result<UserProfileDto>();
        
        IQueryable<User> usersQuery;

        if (Guid.TryParse(idOrUsername, out var id))
            usersQuery = _dbContext.Users.Where(user => user.Id == id);
        else
            usersQuery = _dbContext.Users.Where(user => user.Username == idOrUsername);

        var user = await usersQuery
            .Select(user => new
            {
                user.Id,
                user.Username,
            
                user.UserProfile,
            })
            .FirstOrDefaultAsync();

        if (user == null || user.UserProfile == null)
            return result.Failed();

        if (dto.Full)
            result.Data = new UserProfileFullDto(user.Id, user.Username, user.UserProfile.Name, user.UserProfile.Surname, user.UserProfile.Gender, user.UserProfile.Country, user.UserProfile.BirthDate, user.UserProfile.AvatarPhotoId, user.UserProfile.Color);
        else
            result.Data = new UserProfileDto(user.Id, user.Username, user.UserProfile.Name, user.UserProfile.Surname, user.UserProfile.Gender, user.UserProfile.Country, user.UserProfile.BirthDate, user.UserProfile.AvatarPhotoId, user.UserProfile.Color);

        return result;
    }

    public async Task<Result<string>> PostPhotoAsync(UserProfile userProfile, byte[] imageBytes)
    {
        var result = new Result<string>();
        
        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Users", userProfile.Id.ToString() , "Images");

        var photoId = Guid.NewGuid().ToString();
        
        var fileName = $"{photoId}.png";
        
        var fullPath = Path.Combine(folderPath, fileName);

        Directory.CreateDirectory(folderPath);

        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await stream.WriteAsync(imageBytes);
        }

        return result.Successful().WithData(photoId);
    }

    public async Task<Result<byte[]>> GetPhotoAsync(string userId, string photoId)
    {
        var result = new Result<byte[]>();
        
        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Users", userId, "Images");

        var fileName = $"{photoId}.png";
        
        var fullPath = Path.Combine(folderPath, fileName);

        try
        {
            var bytes = await File.ReadAllBytesAsync(fullPath);
            
            return result.Successful().WithData(bytes);
        }
        catch (Exception ex)
        {
            return result.Failed().WithException(ex);
        }
    }

    public async Task<Result> UpdateAvatarAsync(UserProfile userProfile, string photoId)
    {
        var result = new Result();
        
        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Users", userProfile.Id.ToString() , "Images");

        var fileName = $"{photoId}.png";
        
        var fullPath = Path.Combine(folderPath, fileName);

        if (!File.Exists(fullPath))
            return result.Failed().WithError("Image not found", "imgNotFound");

        var previousAvatar = userProfile.AvatarPhotoId;

        userProfile.AvatarPhotoId = photoId;

        await _dbContext.SaveChangesAsync();

        return result.Successful();
    }

    public async Task<Result> DeleteAvatarAsync(UserProfile userProfile, bool deleteImage)
    {
        var result = new Result();

        var avatar = userProfile.AvatarPhotoId;

        if (avatar == null)
            return result.Failed().WithError("User doesn't have an avatar!", "noAvatar");

        userProfile.AvatarPhotoId = null;

        if (!deleteImage)
            return result.Successful();
        
        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Users", userProfile.Id.ToString() , "Images");

        var fileName = $"{avatar}.png";
        
        var fullPath = Path.Combine(folderPath, fileName);

        try
        {
            File.Delete(fullPath);
        }
        catch (Exception e)
        {
            
        }
        
        return result.Successful();
    }
}