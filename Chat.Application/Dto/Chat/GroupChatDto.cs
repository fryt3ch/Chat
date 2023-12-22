using Chat.Application.Dto.UserProfile;
using Chat.Application.Enums;

namespace Chat.Application.Dto.Chat;

public abstract record ChatDto(
    Guid Id, 
    ChatType ChatType, 
    DateTime CreatedAt,
    DateTime LastReadMessageSentAt,
    int UnreadMessagesAmount,
    ChatMessageDto? LastMessage
);

public record GroupChatDto(
    Guid Id, 
    DateTime CreatedAt,
    DateTime LastReadMessageSentAt,
    int UnreadMessagesAmount,
    ChatMessageDto? LastMessage,
    
    ProfileColor Color,
    string? PhotoId,
    string DisplayName
): ChatDto(Id, ChatType.Group, CreatedAt, LastReadMessageSentAt, UnreadMessagesAmount, LastMessage);

public record UserChatDto(
    Guid Id, 
    DateTime CreatedAt,
    DateTime LastReadMessageSentAt,
    int UnreadMessagesAmount,
    ChatMessageDto? LastMessage,
    
    UserProfileDto UserProfile
): ChatDto(Id, ChatType.User, CreatedAt, LastReadMessageSentAt, UnreadMessagesAmount, LastMessage);