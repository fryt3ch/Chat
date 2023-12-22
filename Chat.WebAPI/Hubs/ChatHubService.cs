using Chat.Application.Dto.Chat;
using Chat.Application.Interfaces.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Chat.WebAPI.Hubs;

public class ChatHubService : IChatHubService
{
    private readonly IHubContext<ChatHub, IChatHubClient> _hubContext;

    public ChatHubService(IHubContext<ChatHub, IChatHubClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task MessageNew(IEnumerable<string> userIds, MessagesNewDto dto)
    {
        await _hubContext.Clients.Users(userIds).MessageNew(dto);
    }

    public async Task MessageDeleted(IEnumerable<string> userIds, DeleteChatMessagesDto dto)
    {
        await _hubContext.Clients.Users(userIds).MessageDeleted(dto);
    }

    public async Task MessageEdited(IEnumerable<string> userIds, EditChatMessageDto dto)
    {
        await _hubContext.Clients.Users(userIds).MessageEdited(dto);
    }

    public async Task ChatRead(string userId, ChatReadDto dto)
    {
        await _hubContext.Clients.User(userId).ChatRead(dto);
    }

    public async Task SetUserTypingState(IEnumerable<string> userIds, SetUserTypingStateDto dto)
    {
        await _hubContext.Clients.Users(userIds).SetUserTypingState(dto);
    }

    public async Task ChatNew(IEnumerable<string> userIds, ChatJoinDto dto)
    {
        await _hubContext.Clients.Users(userIds).ChatNew(dto);
    }

    public async Task ChatDeleted(IEnumerable<string> userIds, DeleteChatDto dto)
    {
        await _hubContext.Clients.Users(userIds).ChatDeleted(dto);
    }

    public async Task ChatCleared(IEnumerable<string> userIds, ClearChatDto dto)
    {
        await _hubContext.Clients.Users(userIds).ChatCleared(dto);
    }

    public async Task MessagePinned(IEnumerable<string> userIds, ChatMessagePinnedDto dto)
    {
        await _hubContext.Clients.Users(userIds).MessagePinned(dto);
    }

    public async Task MessageUnpinned(IEnumerable<string> userIds, ChatMessageUnpinnedDto dto)
    {
        await _hubContext.Clients.Users(userIds).MessageUnpinned(dto);
    }
}