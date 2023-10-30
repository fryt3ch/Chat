using Chat.Application.Constants.Identity;
using Chat.Application.Dto.Chat;
using Chat.Application.Entities;
using Chat.Application.Helpers;
using Chat.Application.Interfaces;
using Chat.Application.Interfaces.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Chat.WebAPI.Hubs;

[Authorize]
public class ChatHub : Hub<IChatHubClient>
{
    private readonly IChatService _chatService;

    public ChatHub(IChatService chatService)
    {
        _chatService = chatService;
    }

    public async Task SendPrivateMessage(SendChatMessageRequestDto sendChatMessageRequestDto)
    {
        /*if (Context.UserIdentifier is null)
            return;
        
        var chatMessage = new ChatMessage()
        {
            SenderId = Guid.Parse(Context.UserIdentifier),
            ReceiverId = sendPrivateMessageDto.ReceiverId,
            Content = sendPrivateMessageDto.Content,
        };
        
        await _chatService.SendMessage(chatMessage);

        var privateMessageDto = new ChatMessageDto(chatMessage.Id, chatMessage.SenderId, chatMessage.Content, chatMessage.SentAt,
            chatMessage.ReceivedAt, chatMessage.WatchedAt);
        
        await Clients.User(privateMessageDto.UserId.ToString()).SendMessage(privateMessageDto);*/
    }

    public override Task OnConnectedAsync()
    {
        var userIdClaim = Context.User!.FindFirst(IdentityConstants.UserIdClaimType);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            Context.Abort();
            
            return Task.CompletedTask;
        }
        
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var userIdClaim = Context.User!.FindFirst(IdentityConstants.UserIdClaimType);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            Context.Abort();
            
            return Task.CompletedTask;
        }
        
        return base.OnDisconnectedAsync(exception);
    }
}