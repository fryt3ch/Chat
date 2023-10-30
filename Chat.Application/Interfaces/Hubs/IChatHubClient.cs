using Chat.Application.Dto.Chat;

namespace Chat.Application.Interfaces.Hubs;

public interface IChatHubClient
{
    Task SendMessage(SendChatMessageDto dto);
    Task DeleteMessages(DeleteChatMessagesDto dto);
    Task SetUserTypingState(SetUserTypingStateDto dto);
}