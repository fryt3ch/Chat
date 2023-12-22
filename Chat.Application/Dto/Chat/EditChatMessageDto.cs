namespace Chat.Application.Dto.Chat;

public record EditChatMessageDto(Guid ChatId, ChatMessageDto MessageDto);

public record EditChatMessageRequestDto(Guid ChatId, Guid MessageId, string Content);