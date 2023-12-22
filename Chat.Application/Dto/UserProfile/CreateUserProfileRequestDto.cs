using Chat.Domain.Enums;
using FluentValidation;

namespace Chat.Application.Dto.UserProfile;

public record CreateUserProfileRequestDto(string Name, string Surname, DateTime BirthDate, Country Country);

public class CreateUserProfileRequestDtoValidator : AbstractValidator<CreateUserProfileRequestDto>
{
    public CreateUserProfileRequestDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(15);

        RuleFor(x => x.Surname)
            .NotNull();
    }
}