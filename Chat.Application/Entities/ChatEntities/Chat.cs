using Chat.Application.Entities.IdentityEntities;
using Chat.Application.Enums;

namespace Chat.Application.Entities.ChatEntities;

public class Chat
{
    public Guid Id { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<UserChat> UserChats { get; set; } = new List<UserChat>();
    public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
    
    public Guid? LastMessageId { get; set; }
    public ChatMessage? LastMessage { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public ChatType ChatType { get; set; }
}