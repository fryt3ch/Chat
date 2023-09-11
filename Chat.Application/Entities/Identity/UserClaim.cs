using System.Security.Claims;
using Chat.Domain.Common;

namespace Chat.Application.Entities.Identity;

public class UserClaim : IEntity<Guid>
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }

    private string Type { get; set; }
    private string Value { get; set; }
    
    public Claim ToClaim()
    {
        return new Claim(Type, Value);
    }
    
    public void InitializeFromClaim(Claim other)
    {
        Type = other.Type;
        Value = other.Value;
    }
}