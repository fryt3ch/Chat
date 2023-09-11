using Chat.Application.Constants.Identity;
using Chat.Application.Dto;
using Chat.Application.Dto.Identity;
using Chat.Application.Entities;
using Chat.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chat.WebAPI.Controllers;

[Route("api/user/profile")]
[ApiController]
public class UserProfileController : ControllerBase
{
    private readonly IUserProfileManager _userProfileManager;

    public UserProfileController(IUserProfileManager userProfileManager)
    {
        _userProfileManager = userProfileManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync(string? username = null)
    {
        UserProfile? userProfile = null;

        if (username != null)
        {
            userProfile = await _userProfileManager.GetByUsernameAsync(username);
        }
        else
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = Guid.Parse(User.Claims.Where(x => x.Type == IdentityConstants.UserIdClaimType).First().Value);

                userProfile = await _userProfileManager.GetByUserIdAsync(userId);
            }
        }

        if (userProfile == null)
            return NotFound();

        var realUsername = await _userProfileManager.GetUsernameAsync(userProfile);

        var userProfileDto =
            new UserProfileDto(realUsername, userProfile.Name, userProfile.Surname, userProfile.BirthDate);

        return Ok(userProfileDto);
    }
}