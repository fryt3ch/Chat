namespace Chat.Application.Dto.Chat;

public record ChatLastMessagePreviewDto(Guid Id, Guid UserId, string Content, DateTime SentAt);