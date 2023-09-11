using Chat.Application.Dto.Chat;
using Chat.Application.Interfaces.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Chat.WebAPI.Hubs;

[Authorize]
public class ChatHub : Hub<IChatHubClient>
{
    public async Task SendPrivate(ChatMessageDto chatMessageDto)
    {
        await Clients.All.SendPrivate();
    }

    public override Task OnConnectedAsync()
    {
        return Task.CompletedTask;
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        return Task.CompletedTask;
    }
}