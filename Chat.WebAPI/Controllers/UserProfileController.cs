using Chat.Application.Constants.Identity;
using Chat.Application.Dto;
using Chat.Application.Dto.UserProfile;
using Chat.Application.Entities;
using Chat.Application.Entities.UserProfileEntities;
using Chat.Application.Interfaces;
using Chat.Domain.Common.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chat.WebAPI.Controllers;

[ApiController]
[Route("api/user/{id}/profile")]
public class UserProfileController : ControllerBase
{
    private readonly IUserProfileManager _userProfileManager;

    public UserProfileController(IUserProfileManager userProfileManager)
    {
        _userProfileManager = userProfileManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync(string id, [FromQuery] UserProfileRequestDto dto)
    {
        var result = await _userProfileManager.GetUserProfileAsync(id, dto);

        return Ok(result);
    }

    [HttpPost("~/api/user/profile")]
    [Authorize]
    public async Task<IActionResult> CreateAsync([FromBody] CreateUserProfileRequestDto createUserProfileRequestDto)
    {
        var userIdClaim = User.FindFirst(IdentityConstants.UserIdClaimType);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        var userProfile = await _userProfileManager.GetByUserIdAsync(userId);

        if (userProfile != null)
            return Forbid();

        userProfile = new UserProfile()
        {
            Id = userId,
            Name = createUserProfileRequestDto.Name,
            Surname = createUserProfileRequestDto.Surname,
            BirthDate = createUserProfileRequestDto.BirthDate,
            Country = createUserProfileRequestDto.Country,
        };

        var result = await _userProfileManager.CreateAsync(userProfile);

        return Ok(result);
    }

    [HttpPost("~/api/user/profile/photo"), RequestSizeLimit(5 * 1024 * 1024)]
    [Authorize]
    public async Task<IActionResult> PostPhotoAsync([FromForm] PostPhotoRequestDto postPhotoRequestDto)
    {
        var userIdClaim = User.FindFirst(IdentityConstants.UserIdClaimType);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();
        
        var userProfile = await _userProfileManager.GetByUserIdAsync(userId);

        if (userProfile == null)
            return NotFound();

        var file = Request.Form.Files.FirstOrDefault();

        if (file == null)
            return BadRequest();

        Result result;
        
        using (var binaryReader = new BinaryReader(file.OpenReadStream()))
        {
            var imageBytes = binaryReader.ReadBytes((int)file.Length);

            result = await _userProfileManager.PostPhotoAsync(userProfile, imageBytes);
        }

        return Ok(result);
    }

    [HttpGet("photo/{photoId}")]
    public async Task<IActionResult> GetPhotoAsync(string id, string photoId)
    {
        var result = await _userProfileManager.GetPhotoAsync(id, photoId);
        
        if (result.Succeeded)
            return File(result.Data, "image/jpeg");

        if (result.Exception is not null)
        {
            if (result.Exception is FileNotFoundException or DirectoryNotFoundException)
            {
                return NotFound();
            }
        }

        return BadRequest();
    }
    
    [HttpPut("~/api/user/profile/photo/avatar")]
    [Authorize]
    public async Task<IActionResult> PutAvatarAsync([FromBody]UpdateUserProfileAvatarRequestDto updateAvatarRequestDto)
    {
        var userIdClaim = User.FindFirst(IdentityConstants.UserIdClaimType);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();
        
        var userProfile = await _userProfileManager.GetByUserIdAsync(userId);

        if (userProfile == null)
            return NotFound();

        var result = await _userProfileManager.UpdateAvatarAsync(userProfile, updateAvatarRequestDto.PhotoId);

        return Ok(result);
    }
    
    [HttpDelete("~/api/user/profile/photo/avatar")]
    [Authorize]
    public async Task<IActionResult> DeleteAvatarAsync()
    {
        var userIdClaim = User.FindFirst(IdentityConstants.UserIdClaimType);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();
        
        var userProfile = await _userProfileManager.GetByUserIdAsync(userId);

        if (userProfile == null)
            return NotFound();

        var result = await _userProfileManager.DeleteAvatarAsync(userProfile, true);

        return Ok(result);
    }
}