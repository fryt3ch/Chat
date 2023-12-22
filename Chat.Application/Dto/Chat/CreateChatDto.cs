using Chat.Application.Enums;

namespace Chat.Application.Dto.Chat;

public record CreateChatRequestDto(ChatType ChatType, HashSet<Guid> MemberIds);
public record CreateChatDto(Guid ChatId, bool WasCreated);