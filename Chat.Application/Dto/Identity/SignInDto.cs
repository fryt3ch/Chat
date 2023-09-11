namespace Chat.Application.Dto.Identity;

public record SignInDto(string Username, string Password, bool RememberMe);