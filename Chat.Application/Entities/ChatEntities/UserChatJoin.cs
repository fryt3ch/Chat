using Chat.Application.Entities.IdentityEntities;

namespace Chat.Application.Entities.ChatEntities;

public class UserChatJoin
{
    public Guid UserId { get; set; }
    public User User { get; set; }
    
    public Guid ChatId { get; set; }
    public Chat Chat { get; set; }
    
    public DateTime LastReadMessageSentAt { get; set; }
}