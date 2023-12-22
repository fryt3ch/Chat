using Chat.Application.Constants.Identity;
using Chat.Application.Dto.Chat;
using Chat.Application.Entities;
using Chat.Application.Helpers;
using Chat.Application.Interfaces;
using Chat.Application.Interfaces.Hubs;
using Chat.Domain.Common.Results;
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
    
    [HubMethodName("chat.read")]
    public async Task<Result> ChatRead(ChatReadRequestDto dto)
    {
        var result =
            await _chatService.ChatRead(Guid.Parse(Context.UserIdentifier!), dto);
        
        return result;
    }

    [HubMethodName("chat.setTypingState")]
    public async Task<Result> ChatSetTypingState(SetUserTypingStateRequestDto dto)
    {
        var result = await _chatService.SetChatUserTypingState(Guid.Parse(Context.UserIdentifier!), dto);

        return result;
    }
    
    [HubMethodName("chat.create")]
    public async Task<Result> ChatCreate(CreateChatRequestDto dto)
    {
        var result = await _chatService.CreateChat(Guid.Parse(Context.UserIdentifier!), dto);

        return result;
    }
    
    [HubMethodName("chat.clear")]
    public async Task<Result> ChatClear(ClearChatRequestDto dto)
    {
        var result = await _chatService.ClearChat(Guid.Parse(Context.UserIdentifier!), dto);

        return result;
    }
    
    [HubMethodName("chat.delete")]
    public async Task<Result> ChatDelete(DeleteChatRequestDto dto)
    {
        var result = await _chatService.DeleteChat(Guid.Parse(Context.UserIdentifier!), dto);

        return result;
    }
    
    [HubMethodName("chats.get")]
    public async Task<Result> ChatsGet(GetChatsRequestDto dto)
    {
        var result = await _chatService.GetChats(Guid.Parse(Context.UserIdentifier!), dto);

        return result;
    }
    
    [HubMethodName("message.send")]
    public async Task<Result> MessageSent(SendChatMessageRequestDto dto)
    {
        var result = await _chatService.SendChatMessage(Guid.Parse(Context.UserIdentifier!), dto);

        return result;
    }
    
    [HubMethodName("message.edit")]
    public async Task<Result> MessageEdit(EditChatMessageRequestDto dto)
    {
        var result = await _chatService.EditChatMessage(Guid.Parse(Context.UserIdentifier!), dto);

        return result;
    }
    
    [HubMethodName("messages.delete")]
    public async Task<Result> MessagesDelete(DeleteChatMessagesRequestDto dto)
    {
        var result = await _chatService.DeleteChatMessages(Guid.Parse(Context.UserIdentifier!), dto);

        return result;
    }
    
    [HubMethodName("messages.get")]
    public async Task<Result> MessagesGet(GetChatMessagesRequestDto dto)
    {
        var result = await _chatService.GetChatMessages(Guid.Parse(Context.UserIdentifier!), dto);

        return result;
    }
}