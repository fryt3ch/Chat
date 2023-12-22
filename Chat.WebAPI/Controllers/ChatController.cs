using Chat.Application.Constants.Identity;
using Chat.Application.Dto.Chat;
using Chat.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    
    [Route("messages/pinned")]
    [HttpGet]
    public async Task<IActionResult> GetPinnedChatMessages(Guid chatId, [FromQuery] GetChatMessagesRequestDto dto)
    {
        var userIdClaim = User.FindFirst(IdentityConstants.UserIdClaimType);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        var messagesResult =
            await _chatService.GetPinnedChatMessages(userId, chatId, dto);

        if (messagesResult.Error != null)
        {
            if (messagesResult.Error.Code == "chatNotFound")
            {
                return NotFound();
            }
        }
        
        return Ok(messagesResult);
    }
    
    [Route("message/{messageId}/pin")]
    [HttpPost]
    public async Task<IActionResult> PinChatMessage(Guid chatId, Guid messageId)
    {
        var userIdClaim = User.FindFirst(IdentityConstants.UserIdClaimType);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        var result = await _chatService.PinChatMessage(userId, chatId, messageId);

        return Ok(result);
    }
    
    [Route("message/{messageId}/pin")]
    [HttpDelete]
    public async Task<IActionResult> UnpinChatMessage(Guid chatId, Guid messageId)
    {
        var userIdClaim = User.FindFirst(IdentityConstants.UserIdClaimType);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        var result = await _chatService.UnpinChatMessage(userId, chatId, messageId);

        return Ok(result);
    }
}