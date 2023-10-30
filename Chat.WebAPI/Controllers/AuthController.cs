using Chat.Application.Dto.Auth;
using Chat.Application.Entities.IdentityEntities;
using Chat.Application.Interfaces.Identity;
using Chat.Domain.Common.Results;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IResult = Chat.Domain.Common.Results.IResult;

namespace Chat.WebAPI.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ISignInManager _signInManager;
    private readonly IUserManager _userManager;

    public AuthController(ISignInManager signInManager, IUserManager userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }
    
    [HttpPost("signin")]
    public async Task<IActionResult> SignInAsync([FromBody] SignInRequestDto signInRequestDto)
    {
        var result = (new Result()).Failed().WithError("Wrong credentials were provided!", "wrongUsernameOrPassword");
        
        await _signInManager.SignOutAsync();

        var user = await _userManager.FindByUsernameAsync(signInRequestDto.Username);

        if (user == null)
            return Ok(result);

        var signInResult = await _signInManager.PasswordSignInAsync(user, signInRequestDto.Password,
            new AuthenticationProperties() { IsPersistent = signInRequestDto.RememberMe, });

        if (!signInResult.Succeeded)
            return Ok(result);
        
        return Ok(signInResult);
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignUpAsync([FromBody] SignUpRequestDto signUpRequestDto)
    {
        await _signInManager.SignOutAsync();

        var result = await _userManager.CreateAsync(new User()
        {
            Username = signUpRequestDto.Username,
            Password = signUpRequestDto.Password,
            Email = signUpRequestDto.Email,
        });

        return Ok(result);
    }

    [Authorize]
    [HttpPost("signout")]
    public async Task<IActionResult> SignOutAsync()
    {
        await _signInManager.SignOutAsync();

        return Ok();
    }
}