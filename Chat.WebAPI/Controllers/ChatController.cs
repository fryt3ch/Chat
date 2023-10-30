using System.Text.Json;
using System.Text.Json.Serialization;
using Chat.Application.Constants.Identity;
using Chat.Application.Dto.Chat;
using Chat.Application.Entities;
using Chat.Application.Entities.ChatEntities;
using Chat.Application.Enums;
using Chat.Application.Interfaces;
using Chat.Application.Interfaces.Hubs;
using Chat.Domain.Common.Results;
using Chat.WebAPI.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Chat.WebAPI.Controllers;

[Route("api/chat/{chatId}")]
[ApiController]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [Route("~/api/chats/previews")]
    [HttpGet]
    public async Task<IActionResult> GetChatsPreviews([FromQuery] GetChatsPreviewsRequestDto getChatsPreviewsRequestDto)
    {
        var userIdClaim = User.FindFirst(IdentityConstants.UserIdClaimType);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        var userChatsResult = await _chatService.GetChatsPreviews(userId, getChatsPreviewsRequestDto.Offset, getChatsPreviewsRequestDto.Count);
        
        return Ok(userChatsResult);
    }

    [Route("messages")]
    [HttpGet]
    public async Task<IActionResult> GetChatMessages(Guid chatId, [FromQuery] GetChatMessagesRequestDto dto)
    {
        var userIdClaim = User.FindFirst(IdentityConstants.UserIdClaimType);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        var messagesResult =
            await _chatService.GetChatMessages(userId, chatId, dto.Offset, dto.Count, dto.MinDate, dto.MaxDate);

        if (messagesResult.Error != null)
        {
            if (messagesResult.Error.Code == "chatNotFound")
            {
                return NotFound();
            }
        }
        
        return Ok(messagesResult);
    }
    
    [Route("message")]
    [HttpPost]
    public async Task<IActionResult> SendChatMessage(Guid chatId, [FromBody] SendChatMessageRequestDto dto)
    {
        var userIdClaim = User.FindFirst(IdentityConstants.UserIdClaimType);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        var result = await _chatService.SendChatMessage(userId, chatId, dto);

        return Ok(result);
    }
    
    [Route("message/{messageId}")]
    [HttpPatch]
    public async Task<IActionResult> EditChatMessage(Guid chatId, Guid messageId, [FromBody] EditChatMessageRequestDto dto)
    {
        var userIdClaim = User.FindFirst(IdentityConstants.UserIdClaimType);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        var result = await _chatService.EditChatMessage(userId, chatId, messageId, dto);

        return Ok(result);
    }
    
    [Route("message/{messageId}")]
    [HttpDelete]
    public async Task<IActionResult> DeleteChatMessage(Guid chatId, Guid messageId)
    {
        var userIdClaim = User.FindFirst(IdentityConstants.UserIdClaimType);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        var result = await _chatService.DeleteChatMessages(userId, chatId, new HashSet<Guid>() { messageId, });

        return Ok(result);
    }
    
    [Route("messages")]
    [HttpDelete]
    public async Task<IActionResult> DeleteChatMessages(Guid chatId, [FromQuery] DeleteChatMessagesRequestDto dto)
    {
        var userIdClaim = User.FindFirst(IdentityConstants.UserIdClaimType);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        var result = await _chatService.DeleteChatMessages(userId, chatId, dto.MessageIds);

        return Ok(result);
    }
    
    [Route("user-typing-state")]
    [HttpPut]
    public async Task<IActionResult> SetUserTypingState(Guid chatId, [FromBody] SetUserTypingStateRequestDto dto)
    {
        var userIdClaim = User.FindFirst(IdentityConstants.UserIdClaimType);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        var result = await _chatService.SetChatUserTypingState(userId, chatId, dto.State);

        return Ok(result);
    }
}