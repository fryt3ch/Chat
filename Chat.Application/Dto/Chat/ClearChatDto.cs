namespace Chat.Application.Dto.Chat;

public record ClearChatRequestDto(
    Guid ChatId
);

public record ClearChatDto(
    Guid ChatId,
    int DeletedMessagesAmount
);