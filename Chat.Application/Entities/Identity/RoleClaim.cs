using System.Security.Claims;
using Chat.Domain.Common;

namespace Chat.Application.Entities.Identity;

public class RoleClaim : IEntity<Guid>
{
    public Guid Id { get; set; }
    
    public Guid RoleId { get; set; }
    
    public string Type { get; set; }
    public string Value { get; set; }
    
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