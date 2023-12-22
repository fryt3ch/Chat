using Chat.Application.Dto.Chat;

namespace Chat.Application.Interfaces.Hubs;

public interface IChatHubService
{
    Task MessageNew(IEnumerable<string> userIds, MessagesNewDto dto);

    Task MessageDeleted(IEnumerable<string> userIds, DeleteChatMessagesDto dto);
    
    Task MessageEdited(IEnumerable<string> userIds, EditChatMessageDto dto);
    
    Task MessagePinned(IEnumerable<string> userIds, ChatMessagePinnedDto dto);

    Task MessageUnpinned(IEnumerable<string> userIds, ChatMessageUnpinnedDto dto);
    
    Task ChatRead(string userId, ChatReadDto dto);
    
    Task SetUserTypingState(IEnumerable<string> userIds, SetUserTypingStateDto dto);
    Task ChatNew(IEnumerable<string> userIds, ChatJoinDto dto);
    Task ChatDeleted(IEnumerable<string> userIds, DeleteChatDto dto);
    Task ChatCleared(IEnumerable<string> userIds, ClearChatDto dto);
}