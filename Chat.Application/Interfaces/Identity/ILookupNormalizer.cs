namespace Chat.Application.Interfaces.Identity;

public interface ILookupNormalizer
{
    string NormalizeName(string name);
    
    string NormalizeEmail(string email);
}