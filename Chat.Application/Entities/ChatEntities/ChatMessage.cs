using Chat.Application.Dto.Chat;
using Chat.Application.Entities.IdentityEntities;

namespace Chat.Application.Entities.ChatEntities;

public class ChatMessage
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    public User User { get; set; }
    
    public Guid ChatId { get; set; }
    public Chat Chat { get; set; }
    
    public string Content { get; set; }
    
    public DateTime SentAt { get; set; }
    public DateTime? ReceivedAt { get; set; }
    public DateTime? WatchedAt { get; set; }
}

public static class ChatMessageExtensions
{
    public static ChatMessageDto MapToDto(this ChatMessage message)
    {
        return new ChatMessageDto(message.Id, message.UserId, message.Content, message.SentAt, message.ReceivedAt, message.WatchedAt);
    }
}