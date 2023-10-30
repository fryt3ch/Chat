using System.Diagnostics.CodeAnalysis;
using Chat.Application.Dto.Chat;
using Chat.Application.Dto.UserProfile;
using Chat.Application.Entities;
using Chat.Application.Entities.ChatEntities;
using Chat.Application.Entities.IdentityEntities;
using Chat.Application.Enums;
using Chat.Application.Interfaces;
using Chat.Application.Interfaces.Hubs;
using Chat.Domain.Common.Results;
using Chat.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Chat.Infrastructure.Services;

public class ChatService : IChatService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ITimeService _timeService;
    private readonly IChatHubService _chatHubService;

    public ChatService(ApplicationDbContext dbContext, ITimeService timeService, IChatHubService chatHubService)
    {
        _dbContext = dbContext;
        _timeService = timeService;
        _chatHubService = chatHubService;
    }

    public async Task<Application.Entities.ChatEntities.Chat?> GetChatById(Guid id)
    {
        return await _dbContext.Chats.Where(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Result> SendChatMessage(Guid userId, Guid chatId, SendChatMessageRequestDto dto)
    {
        var result = new Result();

        var user = await _dbContext.Users
            .Where(user => user.Id == userId)
            .Select(user => new
            {
                Chat = user.Chats
                    .Where(y => y.Id == chatId)
                    .Select(chat => new
                    {
                        Users = chat.Users.Select(chatUser => new
                        {
                            chatUser.Id,
                        }),
                    })
                    .FirstOrDefault(),
            })
            .FirstOrDefaultAsync();

        if (user == null)
            return result.Failed();

        if (user.Chat == null)
            return result.Failed().WithError("Chat doesn't exist", "chatNotFound");

        var message = new ChatMessage()
        {
            Id = Guid.NewGuid(),
            SentAt = _timeService.GetCurrentTime(),

            UserId = userId,
            ChatId = chatId,
            
            Content = dto.Content,
        };

        await _dbContext.ChatMessages.AddAsync(message);

        var realChat = new Application.Entities.ChatEntities.Chat() { Id = chatId, };

        _dbContext.Attach(realChat);

        realChat.LastMessage = message;

        await _dbContext.SaveChangesAsync();

        await _chatHubService.SendMessage(user.Chat.Users.Select(x => x.Id.ToString()), new SendChatMessageDto(chatId, new ChatMessageDto(message.Id, userId, message.Content, message.SentAt, message.ReceivedAt, message.WatchedAt)));

        return result.Successful();
    }

    public async Task<Result> EditChatMessage(Guid userId, Guid chatId, Guid messageId, EditChatMessageRequestDto dto)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> DeleteChatMessages(Guid userId, Guid chatId, HashSet<Guid> messageIds)
    {
        var result = new Result();

        var user = await _dbContext.Users
            .Where(user => user.Id == userId)
            .Select(user => new
            {
                Chat = user.Chats
                    .Where(chat => chat.Id == chatId)
                    .Select(chat => new
                    {
                        Users = chat.Users.Select(chatUser => new
                        {
                            chatUser.Id,
                        }),
                        
                        Messages = chat.ChatMessages
                            .Where(chatMessage => messageIds.Contains(chatMessage.Id))
                            .Select(chatMessage => new
                            {
                                chatMessage.Id,
                                chatMessage.UserId,
                            })
                            .ToList(),
                        
                        chat.LastMessageId,
                    })
                    .FirstOrDefault(),
            })
            .FirstOrDefaultAsync();

        if (user == null)
            return result.Failed();
        
        if (user.Chat == null)
            return result.Failed().WithError("Chat doesn't exist", "chatNotFound");
        
        if (user.Chat.Messages.Count == 0)
            return result.Failed().WithError("Messages don't exist", "messagesNotFound");

        var realChat = new Application.Entities.ChatEntities.Chat() { Id = chatId, };
        
        _dbContext.Attach(realChat);

        var deletedIds = new HashSet<Guid>();
        
        foreach (var x in user.Chat.Messages)
        {
            if (x.UserId != userId)
                return result.Failed().WithError("Access forbidden", "forbidden");
            
            var realMessage = new ChatMessage() { Id = x.Id, };
            
            _dbContext.Attach(realMessage);
            
            _dbContext.ChatMessages.Remove(realMessage);

            deletedIds.Add(x.Id);
        }
        
        if (user.Chat.LastMessageId is { } lastMessageId && deletedIds.Contains(lastMessageId))
        {
            var lastMessage = await _dbContext.Entry(realChat).Collection(x => x.ChatMessages).Query()
                .Where(x => !deletedIds.Contains(x.Id))
                .Select(chatMessage => new
                {
                    chatMessage.Id,
                    chatMessage.SentAt,
                })
                .OrderByDescending(x => x.SentAt)
                .FirstOrDefaultAsync();

            if (lastMessage != null)
            {
                realChat.LastMessageId = lastMessage.Id;
            }
        }
        
        await _dbContext.SaveChangesAsync();

        await _chatHubService.DeleteMessages(user.Chat.Users.Select(x => x.Id.ToString()), new DeleteChatMessagesDto(chatId, deletedIds));
        
        return result.Successful();
    }
    
    public async Task<Result<IEnumerable<ChatPreviewDto>>> GetChatsPreviews(Guid userId, int offset, int count)
    {
        var result = new Result<IEnumerable<ChatPreviewDto>>();

        var user = await _dbContext.Users
            .Where(user => user.Id == userId)
            .Select(user => new
            {
                
            })
            .FirstOrDefaultAsync();

        if (user == null)
            return result.Failed().WithError("User doesn't exist", "userNotFound");

        var realUser = new User() { Id = userId };

        _dbContext.Attach(realUser);
        
        var chatsQuery = _dbContext.Entry(realUser).Collection(x => x.Chats).Query()
            .OrderByDescending(chat => chat.LastMessage == null ? chat.CreatedAt : chat.LastMessage.SentAt)
            .Select(chat => new
            {
                chat.Id,
                chat.CreatedAt,
                chat.ChatType,
                chat.LastMessageId,
                LastMessage = chat.LastMessage == null ? null : new
                {
                    chat.LastMessage.Id,
                    chat.LastMessage.SentAt,
                    chat.LastMessage.Content,
                    
                    User = new
                    {
                        chat.LastMessage.User.Id,
                        chat.LastMessage.User.Username,
                    }
                },
                
                Member = chat.Users
                    .Where(chatUser => chatUser.Id != userId)
                    .Select(chatUser => new
                    {
                        chatUser.Id,
                        chatUser.Username,
                        
                        ProfileData = chatUser.UserProfile,
                    })
                    .FirstOrDefault()
            })
            .AsNoTracking();
        
        if (offset != 0)
            chatsQuery = chatsQuery.Skip(offset);

        if (count > 0)
            chatsQuery = chatsQuery.Take(count);

        var chats = await chatsQuery.ToListAsync();

        var chatsPreviews = chats.Select(x =>
        {
            var lastMessagePreview = x.LastMessage is null ? null : new ChatLastMessagePreviewDto(x.LastMessage.Id, x.LastMessage.User.Id, x.LastMessage.Content, x.LastMessage.SentAt);
            
            var dto = new UserChatPreviewDto(x.Id, new UserProfileDto(x.Member.Id, x.Member.Username, x.Member.ProfileData.Name, x.Member.ProfileData.Surname, x.Member.ProfileData.Gender, x.Member.ProfileData.Country, x.Member.ProfileData.BirthDate, x.Member.ProfileData.AvatarPhotoId, x.Member.ProfileData.Color), lastMessagePreview);

            return dto;
        });

        return result.Successful().WithData(chatsPreviews);
    }

    public async Task<Result<IEnumerable<ChatMessageDto>>> GetChatMessages(Guid userId, Guid chatId, int offset, int count, DateTime? minDate, DateTime? maxDate)
    {
        var result = new Result<IEnumerable<ChatMessageDto>>();
        
        var user = await _dbContext.Users
            .Where(user => user.Id == userId)
            .Select(user => new
            {
                Chat = user.Chats
                    .Where(chat => chat.Id == chatId)
                    .Select(chat => new
                    {
                        
                    })
                    .FirstOrDefault(),
            })
            .FirstOrDefaultAsync();

        if (user == null)
            return result.Failed();

        if (user.Chat == null)
            return result.Failed().WithError("Chat doesn't exist", "chatNotFound");

        var realChat = new Application.Entities.ChatEntities.Chat() { Id = chatId, };

        _dbContext.Attach(realChat);

        var messagesQuery = _dbContext.Entry(realChat).Collection(x => x.ChatMessages).Query();

        if (minDate is not null)
            messagesQuery = messagesQuery.Where(x => x.SentAt >= minDate);
        
        if (maxDate is not null)
            messagesQuery = messagesQuery.Where(x => x.SentAt <= maxDate);

        messagesQuery = messagesQuery
            .OrderByDescending(x => x.SentAt);
        
        if (offset > 0)
            messagesQuery = messagesQuery.Skip(offset);

        if (count > 0)
            messagesQuery = messagesQuery.Take(count);

        var messages = await messagesQuery.ToListAsync();
        
        return result.Successful().WithData(messages.Select(x => x.MapToDto()));
    }

    public async Task<Result> SetChatUserTypingState(Guid userId, Guid chatId, ChatUserTypingState state)
    {
        var result = new Result();
        
        var user = await _dbContext.Users
            .Where(user => user.Id == userId)
            .Select(user => new
            {
                Chat = user.Chats
                    .Where(chat => chat.Id == chatId)
                    .Select(chat => new
                    {
                        Users = chat.Users
                            .Select(chatUser => new { chatUser.Id, }),
                    })
                    .FirstOrDefault(),
            })
            .FirstOrDefaultAsync();

        if (user == null)
            return result.Failed();

        if (user.Chat == null)
            return result.Failed().WithError("Chat doesn't exist", "chatNotFound");

        await _chatHubService.SetUserTypingState(user.Chat.Users.Select(x => x.Id.ToString()), new SetUserTypingStateDto(chatId, userId, state));

        return result;
    }
}