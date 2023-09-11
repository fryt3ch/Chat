using Chat.Domain.Common;

namespace Chat.Application.Entities.Identity;

public class User : IEntity<Guid>
{
    public Guid Id { get; set; }
    
    public string Username { get; set; }
    public string NormalizedUsername { get; set; }
    
    public string Password { get; set; }
    
    public string? Email { get; set; }
    public string? NormalizedEmail { get; set; }
    
    public string? PhoneNumber { get; set; }

    public Guid ConcurrencyStamp { get; set; } = Guid.NewGuid();
}