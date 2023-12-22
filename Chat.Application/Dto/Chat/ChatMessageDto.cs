using Chat.Application.Enums;

namespace Chat.Application.Dto.Chat;

public record ChatMessageDto
(
    Guid Id,
    ChatMessageType MessageType,
    Guid ChatId,
    Guid UserId,
    string Content,
    DateTime SentAt,
    DateTime? ReceivedAt,
    DateTime? WatchedAt,
    
    ChatMessageDto? SourceMessageDto,
    Guid? SourceUserId
);