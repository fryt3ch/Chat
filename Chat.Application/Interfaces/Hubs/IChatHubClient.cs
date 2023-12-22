using Chat.Application.Dto.Chat;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Application.Interfaces.Hubs;

public interface IChatHubClient
{
    [HubMethodName("message.new")]
    Task MessageNew(MessagesNewDto dto);
    
    [HubMethodName("messages.deleted")]
    Task MessageDeleted(DeleteChatMessagesDto dto);
    
    [HubMethodName("message.edited")]
    Task MessageEdited(EditChatMessageDto dto);
    
    [HubMethodName("message.pinned")]
    Task MessagePinned(ChatMessagePinnedDto dto);
    
    [HubMethodName("message.unpinned")]
    Task MessageUnpinned(ChatMessageUnpinnedDto dto);
    
    [HubMethodName("chat.read")]
    Task ChatRead(ChatReadDto dto);
    
    [HubMethodName("chat.typingState")]
    Task SetUserTypingState(SetUserTypingStateDto dto);
    
    [HubMethodName("chat.new")]
    Task ChatNew(ChatJoinDto dto);
    
    [HubMethodName("chat.deleted")]
    Task ChatDeleted(DeleteChatDto dto);
    
    [HubMethodName("chat.cleared")]
    Task ChatCleared(ClearChatDto dto);
}