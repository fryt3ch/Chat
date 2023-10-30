using Chat.Application.Dto.Chat;
using Chat.Application.Entities;
using Chat.Application.Entities.ChatEntities;
using Chat.Application.Enums;
using Chat.Domain.Common.Results;

namespace Chat.Application.Interfaces;

public interface IChatService
{
    public Task<Result> SendChatMessage(Guid userId, Guid chatId, SendChatMessageRequestDto dto);
    
    public Task<Result> EditChatMessage(Guid userId, Guid chatId, Guid messageId, EditChatMessageRequestDto dto);
    
    public Task<Result> DeleteChatMessages(Guid userId, Guid chatId, HashSet<Guid> messageIds);

    public Task<Result<IEnumerable<ChatPreviewDto>>> GetChatsPreviews(Guid userId, int offset, int count);

    public Task<Result<IEnumerable<ChatMessageDto>>> GetChatMessages(Guid userId, Guid chatId, int offset, int count, DateTime? minDate, DateTime? maxDate);

    public Task<Result> SetChatUserTypingState(Guid userId, Guid chatId, ChatUserTypingState state);
}