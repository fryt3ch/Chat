using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Validators;

namespace Chat.Application.Validators;

public class EmailValidator<T> : PropertyValidator<T, string?>, IRegularExpressionValidator
{
    public override string Name => "EmailValidator";

    private const string ExpressionString =
        @"^[a-zA-Z0-9!#$%&'*+=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9][a-zA-Z0-9-]*[a-zA-Z0-9]$";

    public override bool IsValid(ValidationContext<T> context, string? value)
    {
        if (value == null)
            return true;
        
        return _regex.IsMatch(value);
    }

    protected override string GetDefaultMessageTemplate(string errorCode)
        => "'{PropertyName}' is not valid e-mail";

    public string Expression => ExpressionString;

    private static readonly Regex _regex = new Regex(ExpressionString, RegexOptions.Compiled);
}