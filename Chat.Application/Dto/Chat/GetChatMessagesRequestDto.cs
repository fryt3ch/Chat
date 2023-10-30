namespace Chat.Application.Dto.Chat;

public record GetChatMessagesRequestDto(int Offset, int Count, DateTime? MinDate, DateTime? MaxDate);