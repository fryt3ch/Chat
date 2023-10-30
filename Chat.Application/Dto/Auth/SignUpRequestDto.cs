using Chat.Application.Validators;
using FluentValidation;

namespace Chat.Application.Dto.Auth;

public record SignUpRequestDto(string Username, string Email, string Password);

public class SignUpRequestDtoValidator : AbstractValidator<SignUpRequestDto>
{
    public SignUpRequestDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotNull()
            .MinimumLength(5)
            .MaximumLength(15);

        RuleFor(x => x.Password)
            .NotNull()
            .MinimumLength(5)
            .MaximumLength(15);

        RuleFor(x => x.Email)
            .NotEmpty()
            .Email();
    }
}