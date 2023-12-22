using Chat.Application.Dto.Chat;
using Chat.Application.Entities;
using Chat.Application.Entities.ChatEntities;
using Chat.Application.Enums;
using Chat.Domain.Common.Results;

namespace Chat.Application.Interfaces;

public interface IChatService
{
    public Task<Result> SendChatMessage(Guid userId, SendChatMessageRequestDto dto);
    
    public Task<Result> EditChatMessage(Guid userId, EditChatMessageRequestDto dto);
    
    public Task<Result> DeleteChatMessages(Guid userId, DeleteChatMessagesRequestDto dto);

    public Task<Result<IEnumerable<ChatDto>>> GetChats(Guid userId, GetChatsRequestDto dto);

    public Task<Result<GetChatMessagesDto>> GetChatMessages(Guid userId, GetChatMessagesRequestDto dto);
    public Task<Result<GetChatMessagesDto>> GetPinnedChatMessages(Guid userId, Guid chatId, GetChatMessagesRequestDto dto);

    public Task<Result> SetChatUserTypingState(Guid userId, SetUserTypingStateRequestDto dto);
    
    public Task<Result> PinChatMessage(Guid userId, Guid chatId, Guid messageId);
    public Task<Result> UnpinChatMessage(Guid userId, Guid chatId, Guid messageId);
    
    public Task<Result> ChatRead(Guid userId, ChatReadRequestDto dto);
    public Task<Result<CreateChatDto>> CreateChat(Guid userId, CreateChatRequestDto dto);
    public Task<Result> ClearChat(Guid userId, ClearChatRequestDto dto);
    public Task<Result> DeleteChat(Guid userId, DeleteChatRequestDto dto);
}