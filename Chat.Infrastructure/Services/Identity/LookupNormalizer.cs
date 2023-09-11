using Chat.Application.Interfaces.Identity;

namespace Chat.Infrastructure.Services.Identity;

public class LookupNormalizer : ILookupNormalizer
{
    public string NormalizeName(string name)
    {
        return name.ToUpperInvariant();
    }

    public string NormalizeEmail(string email)
    {
        return email.ToUpperInvariant();
    }
}