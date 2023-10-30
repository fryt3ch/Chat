namespace Chat.Application.Dto.Chat;

public record DeleteChatMessagesDto(Guid ChatId, IEnumerable<Guid> MessageIds);
public record DeleteChatMessagesRequestDto(HashSet<Guid> MessageIds);