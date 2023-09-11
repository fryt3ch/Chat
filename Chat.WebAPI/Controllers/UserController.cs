using Chat.Application.Dto.Identity;
using Chat.Application.Entities.Identity;
using Chat.Application.Interfaces.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chat.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserManager _userManager;

    public UserController(IUserManager userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAsync(Guid? userId = null, string? username = null)
    {
        User? user = null;
        
        if (userId == null)
        {
            if (username == null)
            {
                user = await _userManager.FindByUsernameAsync(User.Identity!.Name!);
            }
            else
            {
                user = await _userManager.FindByUsernameAsync(username);
            }
        }
        else
        {
            
        }
        
        if (user == null)
            return NotFound();

        var userDto = new UserDto(user.Id, user.Username);

        return Ok(userDto);
    }
}