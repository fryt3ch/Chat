using BCrypt.Net;
using Chat.Application.Interfaces.Identity;
using Chat.Domain.Common.Results;

namespace Chat.Infrastructure.Services.Identity;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string plainText)
    {
        var hashedText = BCrypt.Net.BCrypt.HashPassword(plainText);

        return hashedText;
    }

    public Result Verify(string plainText, string hashedText)
    {
        var result = new Result();
        
        var verifyResult = BCrypt.Net.BCrypt.Verify(plainText, hashedText, false, HashType.SHA384);

        if (verifyResult)
            return result;

        return result.Failed();
    }
}