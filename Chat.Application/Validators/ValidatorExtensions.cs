using FluentValidation;

namespace Chat.Application.Validators;

public static class ValidatorExtensions
{
    public static IRuleBuilderOptions<T, string> Email<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.SetValidator(new EmailValidator<T>());
    }
}