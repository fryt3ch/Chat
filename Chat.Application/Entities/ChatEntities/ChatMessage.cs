using Chat.Application.Dto.Chat;
using Chat.Application.Entities.IdentityEntities;
using Chat.Application.Enums;

namespace Chat.Application.Entities.ChatEntities;

public class ChatMessage : ICloneable
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    public User User { get; set; }
    
    public Guid ChatId { get; set; }
    public Chat Chat { get; set; }
    
    public ulong ChatMessageId { get; set; }
    
    public string Content { get; set; }
    public string NormalizedContent { get; set; }
    
    public DateTime SentAt { get; set; }
    public DateTime? ReceivedAt { get; set; }
    public DateTime? ReadAt { get; set; }
    
    public DateTime? PinnedAt { get; set; }
    public Guid? PinnedById { get; set; }
    public User? PinnedBy { get; set; }
    
    public Guid? SourceUserId { get; set; }
    public User? SourceUser { get; set; }
    
    public Guid? SourceMessageId { get; set; }
    public ChatMessage? SourceMessage { get; set; }
    
    public uint ReadCounter { get; set; }
    
    public ChatMessageType MessageType { get; set; }
    
    public object Clone()
    {
        return this.MemberwiseClone();
    }
}

public static class ChatMessageExtensions
{
    public static ChatMessageDto MapToDto(this ChatMessage message)
    {
        return new ChatMessageDto(
            message.Id, 
            message.MessageType, 
            message.ChatId, 
            message.UserId, 
            message.Content, 
            message.SentAt, 
            message.ReceivedAt, 
            message.ReadAt, 
            message.SourceMessage?.MapToDto(), 
            message.SourceUserId
        );
    }
}