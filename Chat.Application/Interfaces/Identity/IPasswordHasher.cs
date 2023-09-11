using Chat.Domain.Common.Results;

namespace Chat.Application.Interfaces.Identity;

public interface IPasswordHasher
{
    public string Hash(string plainText);

    public Result Verify(string plainText, string hashedText);
}