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

    public async Task SendMessage(IEnumerable<string> userIds, SendChatMessageDto dto)
    {
        await _hubContext.Clients.Users(userIds).SendMessage(dto);
    }

    public async Task DeleteMessages(IEnumerable<string> userIds, DeleteChatMessagesDto dto)
    {
        await _hubContext.Clients.Users(userIds).DeleteMessages(dto);
    }
    
    public async Task SetUserTypingState(IEnumerable<string> userIds, SetUserTypingStateDto dto)
    {
        await _hubContext.Clients.Users(userIds).SetUserTypingState(dto);
    }
}