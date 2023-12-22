using Chat.Application.Entities.IdentityEntities;
using Chat.Application.Enums;

namespace Chat.Application.Entities.ChatEntities;

public abstract class Chat
{
    public Guid Id { get; set; }
    
    public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
    
    public Guid? LastMessageId { get; set; }
    public ChatMessage? LastMessage { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public ChatType ChatType { get; set; }
    
    public ICollection<User> Users { get; set; } = new List<User>();
    
    public ICollection<UserChatJoin> UserChatJoins { get; set; } = new List<UserChatJoin>();
}

public class GroupChat : Chat
{
    
}

public class UserChat : Chat
{
    public Guid UserFirstId { get; set; }
    public User UserFirst { get; set; }
    
    public Guid UserSecondId { get; set; }
    public User UserSecond { get; set; }
}