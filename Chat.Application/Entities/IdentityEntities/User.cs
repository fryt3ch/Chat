using Chat.Application.Entities.ChatEntities;
using Chat.Application.Entities.UserProfileEntities;
using Chat.Domain.Common;

namespace Chat.Application.Entities.IdentityEntities;

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
    
    public UserProfile? UserProfile { get; set; }

    public ICollection<ChatEntities.Chat> Chats { get; set; } = new List<ChatEntities.Chat>();
    public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
    public ICollection<UserChat> UserChats { get; set; } = new List<UserChat>();
}