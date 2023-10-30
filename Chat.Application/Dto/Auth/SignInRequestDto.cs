using FluentValidation;

namespace Chat.Application.Dto.Auth;

public record SignInRequestDto(string Username, string Password, bool RememberMe);

public class SignInRequestDtoValidator : AbstractValidator<SignInRequestDto>
{
    public SignInRequestDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}