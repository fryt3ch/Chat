using Chat.Application.Dto.Chat;

namespace Chat.Application.Interfaces.Hubs;

public interface IChatHubService
{
    Task SendMessage(IEnumerable<string> userIds, SendChatMessageDto dto);

    Task DeleteMessages(IEnumerable<string> userIds, DeleteChatMessagesDto dto);
    
    Task SetUserTypingState(IEnumerable<string> userIds, SetUserTypingStateDto dto);
}