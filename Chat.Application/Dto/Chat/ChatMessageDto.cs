namespace Chat.Application.Dto.Chat;

public record ChatMessageDto
(
    Guid Id,
    Guid UserId,
    string Content,
    DateTime SentAt,
    DateTime? ReceivedAt,
    DateTime? WatchedAt
);