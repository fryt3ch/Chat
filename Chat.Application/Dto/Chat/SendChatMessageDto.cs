namespace Chat.Application.Dto.Chat;

public record SendChatMessageDto(Guid ChatId, ChatMessageDto MessageDto);

public record SendChatMessageRequestDto(string Content);