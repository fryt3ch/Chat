using Chat.Application.Dto.Identity;
using Chat.Application.Entities.Identity;
using Chat.Application.Interfaces.Identity;
using Chat.Domain.Common.Results;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chat.WebAPI.Controllers;

[Route("api/[controller]/[action]")]
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
    
    [HttpPost]
    public async Task<IActionResult> SignInAsync([FromBody] SignInDto signInDto)
    {
        var result = (new Result()).Failed().WithError("Wrong data was provided!", "wrongDataProvided");
        
        await _signInManager.SignOutAsync();

        var user = await _userManager.FindByUsernameAsync(signInDto.Username);

        if (user == null)
            return Ok(result);

        var signInResult = await _signInManager.PasswordSignInAsync(user, signInDto.Password,
            new AuthenticationProperties() { IsPersistent = signInDto.RememberMe, });

        if (!signInResult.Succeeded)
            return Ok(result);
        
        return Ok(signInResult);
    }

    [HttpPost]
    public async Task<IActionResult> SignUpAsync([FromBody] SignUpDto signUpDto)
    {
        await _signInManager.SignOutAsync();

        var result = await _userManager.CreateAsync(new User()
        {
            Username = signUpDto.Username,
            Password = signUpDto.Password,
            Email = signUpDto.Email,
        });

        return Ok(result);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> SignOutAsync()
    {
        await _signInManager.SignOutAsync();

        return Ok();
    }
}