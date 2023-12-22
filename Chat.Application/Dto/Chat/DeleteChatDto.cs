namespace Chat.Application.Dto.Chat;

public record DeleteChatRequestDto(
    Guid ChatId
);

public record DeleteChatDto(
    Guid ChatId
);