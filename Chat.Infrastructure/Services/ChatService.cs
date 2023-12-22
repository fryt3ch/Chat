using Chat.Application.Dto.Chat;
using Chat.Application.Entities.ChatEntities;
using Chat.Application.Entities.IdentityEntities;
using Chat.Application.Enums;
using Chat.Application.Interfaces;
using Chat.Application.Interfaces.Hubs;
using Chat.Application.Interfaces.Identity;
using Chat.Domain.Common.Results;
using Chat.Infrastructure.Database;
using Chat.Infrastructure.Database.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Chat.Infrastructure.Services;

public class ChatService : IChatService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ITimeService _timeService;
    private readonly IChatHubService _chatHubService;
    private readonly ILookupNormalizer _lookupNormalizer;

    public ChatService(ApplicationDbContext dbContext, ITimeService timeService, IChatHubService chatHubService, ILookupNormalizer lookupNormalizer)
    {
        _dbContext = dbContext;
        _timeService = timeService;
        _chatHubService = chatHubService;
        _lookupNormalizer = lookupNormalizer;
    }

    public async Task<Application.Entities.ChatEntities.Chat?> GetChatById(Guid id)
    {
        return await _dbContext.Chats.Where(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Result> SendChatMessage(Guid userId, SendChatMessageRequestDto dto)
    {
        var result = new Result();

        var user = await _dbContext.Users
            .Where(user => user.Id == userId)
            .Select(user => new
            {
                UserChat = user.Chats
                    .Where(chat => chat.Id == dto.ChatId)
                    .Select(chat => new
                    {
                        Chat = new
                        {
                            chat.Id,
                            chat.ChatType,
                            
                            Members = chat.ChatType == ChatType.Group ? ((GroupChat)chat).Users.Select(member => new
                            {
                               member.Id,
                            }).ToList() : null,
                            
                            OppositeUser = chat.ChatType == ChatType.User ? new
                            {
                                ((UserChat)chat).GetOppositeUser(userId).Id,
                            } : null,
                        
                            QuotedMessage = dto.QuotedMessageId != null ? chat.ChatMessages
                                .Where(message => message.Id == dto.QuotedMessageId)
                                .FirstOrDefault() : null,
                            
                            ForwardedMessages = dto.ForwardedMessages != null ? _dbContext.Chats
                                .Where(x => x.Id == dto.SourceChatId!)
                                .Select(x => new
                                {
                                    Messages = x.ChatMessages
                                        .Where(x => dto.ForwardedMessages.Contains(x.Id))
                                        .Select(x => new
                                        {
                                            x.Id,
                                            x.Content,
                                            x.SentAt,
                                            
                                            x.UserId,
                                        })
                                        .ToList(),
                                })
                                .FirstOrDefault()
                                : null,
                            
                            LastMessage = chat.LastMessageId == null ? null : new
                            {
                                Id = chat.LastMessageId,
                                chat.LastMessage!.ChatMessageId,
                            },
                        },
                    })
                    .FirstOrDefault(),
            })
            .FirstOrDefaultAsync();

        if (user == null)
            return result.Failed();

        var chat = user.UserChat?.Chat;

        if (chat == null)
            return result.Failed().WithError("Chat doesn't exist", "chatNotFound");

        if (dto.QuotedMessageId != null)
        {
            if (chat.QuotedMessage == null)
                return result.Failed();
        }
        else if (dto.ForwardedMessages != null)
        {
            if (chat.ForwardedMessages == null || chat.ForwardedMessages.Messages.Count == 0)
                return result.Failed();
        }

        var latestChatMessageId = chat.LastMessage?.ChatMessageId ?? 0;

        var currentTime = _timeService.GetCurrentTime();
        var lastMessageTime = currentTime;

        var message = new ChatMessage()
        {
            Id = Guid.NewGuid(),
            ChatId = chat.Id,
            ChatMessageId = ++latestChatMessageId,
            
            SentAt = lastMessageTime,

            UserId = userId,
            
            Content = dto.Content,
            NormalizedContent = _lookupNormalizer.NormalizeName(dto.Content),
        };

        var messagesToAdd = new List<ChatMessage>() { message, };

        if (dto.QuotedMessageId != null)
        {
            message.MessageType = ChatMessageType.Quoted;
            
            message.SourceMessageId = dto.QuotedMessageId;
        }
        else if (chat.ForwardedMessages != null)
        {
            foreach (var x in chat.ForwardedMessages.Messages.OrderBy(x => x.SentAt))
            {
                var forwardedMessage = new ChatMessage()
                {
                    Id = Guid.NewGuid(),
                    ChatId = chat.Id,
                    ChatMessageId = ++latestChatMessageId,
                    
                    MessageType = ChatMessageType.Forwarded,
                    SentAt = (lastMessageTime = lastMessageTime.AddMilliseconds(1)),

                    UserId = userId,

                    Content = x.Content,
                    
                    SourceMessageId = x.Id,
                    SourceUserId = x.UserId,
                };
                
                messagesToAdd.Add(forwardedMessage);
            }
        }
        
        await _dbContext.ChatMessages.AddRangeAsync(messagesToAdd);

        var realUserChat = new UserChatJoin() { ChatId = chat.Id, UserId = userId, Chat = new UserChat() { Id = chat.Id, } };

        _dbContext.Attach(realUserChat);

        var lastMessage = messagesToAdd.Last();

        realUserChat.Chat.LastMessageId = lastMessage.Id;
        realUserChat.LastReadMessageSentAt = lastMessage.SentAt;
        
        await _dbContext.SaveChangesAsync();
        
        var recipients = chat.ChatType == ChatType.User ? new List<string>() { userId.ToString(), chat.OppositeUser!.Id.ToString(), } : chat.Members.Select(x => x.Id.ToString()).ToList();
        
        await _chatHubService.MessageNew(recipients, new MessagesNewDto(messagesToAdd.Select(x => x.MapToDto()).ToList()));

        return result.Successful();
    }

    public async Task<Result> EditChatMessage(Guid userId, EditChatMessageRequestDto dto)
    {
        var result = new Result();

        var user = await _dbContext.Users
            .Where(user => user.Id == userId)
            .Select(user => new
            {
                Chat = user.Chats
                    .Where(y => y.Id == dto.ChatId)
                    .Select(chat => new
                    {
                        Users = chat.Users.Select(chatUser => new
                        {
                            chatUser.Id,
                        }),
                        
                        Message = chat.ChatMessages
                            .Where(x => x.Id == dto.MessageId)
                            .Select(x => x.ProjectToDto())
                            .FirstOrDefault(),
                    })
                    .FirstOrDefault(),
            })
            .FirstOrDefaultAsync();
        
        if (user == null)
            return result.Failed();

        if (user.Chat == null)
            return result.Failed().WithError("Chat doesn't exist", "chatNotFound");

        var message = user.Chat.Message;
        
        if (message == null)
            return result.Failed().WithError("Message doesn't exist", "messageNotFound");

        if (message.UserId != userId)
            return result.Failed().WithError("not allowed", "notAllowed");

        var realMessage = new ChatMessage() { Id = message.Id, };

        _dbContext.Attach(realMessage);

        realMessage.Content = dto.Content;
        realMessage.NormalizedContent = _lookupNormalizer.NormalizeName(dto.Content);

        await _dbContext.SaveChangesAsync();

        var messageDto = message with
        {
            Content = realMessage.Content,
        };

        await _chatHubService.MessageEdited(user.Chat.Users.Select(x => x.Id.ToString()),
            new EditChatMessageDto(dto.ChatId, messageDto));

        return result;
    }

    public async Task<Result> DeleteChatMessages(Guid userId, DeleteChatMessagesRequestDto dto)
    {
        var result = new Result();

        var user = await _dbContext.Users
            .Where(user => user.Id == userId)
            .Select(user => new
            {
                Chat = user.Chats
                    .Where(chat => chat.Id == dto.ChatId)
                    .Select(chat => new
                    {
                        chat.Id,
                        chat.ChatType,
                        
                        Members = chat.ChatType == ChatType.Group ? ((GroupChat)chat).Users.Select(member => new
                        {
                            member.Id,
                        }).ToList() : null,
                        
                        OppositeUser = chat.ChatType == ChatType.User ? new
                        {
                            ((UserChat)chat).GetOppositeUser(userId).Id,
                        } : null,
                        
                        MessagesToDelete = chat.ChatMessages
                            .Where(chatMessage => dto.MessageIds.Contains(chatMessage.Id))
                            .Select(chatMessage => new
                            {
                                chatMessage.Id,
                                chatMessage.ChatMessageId,
                                chatMessage.UserId,
                            })
                            .ToList(),
                        
                        chat.LastMessageId,
                    })
                    .FirstOrDefault(),
            })
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (user == null)
            return result.Failed();

        var chat = user.Chat;
        
        if (chat == null)
            return result.Failed().WithError("Chat doesn't exist", "chatNotFound");

        if (chat.MessagesToDelete.Count == 0)
            return result.Successful().WithMessage("No messages were found!");

        var realChat = new UserChat() { Id = chat.Id, };
        
        _dbContext.Attach(realChat);

        var deletedMessages = new List<ChatMessage>();
        var deletedMessageIds = new HashSet<Guid>();
        
        foreach (var x in chat.MessagesToDelete)
        {
            if (x.UserId != userId)
                return result.Failed().WithError("Access forbidden", "forbidden");
            
            var realMessage = new ChatMessage() { Id = x.Id, ChatMessageId = x.ChatMessageId, };
            
            _dbContext.Attach(realMessage);
            
            _dbContext.ChatMessages.Remove(realMessage);

            deletedMessages.Add(realMessage);
            deletedMessageIds.Add(realMessage.Id);
        }
        
        if (chat.LastMessageId is { } lastMessageId && deletedMessageIds.Contains(lastMessageId))
        {
            var lastMessage = await _dbContext.Entry(realChat).Collection(x => x.ChatMessages).Query()
                .Where(x => !deletedMessageIds.Contains(x.Id))
                .Select(chatMessage => new
                {
                    chatMessage.Id,
                    chatMessage.ChatMessageId,
                })
                .OrderByDescending(x => x.ChatMessageId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (lastMessage != null)
            {
                realChat.LastMessageId = lastMessage.Id;
            }
        }
        
        await _dbContext.SaveChangesAsync();
        
        var recipients = chat.ChatType == ChatType.User ? new List<string>() { userId.ToString(), chat.OppositeUser!.Id.ToString(), } : chat.Members.Select(x => x.Id.ToString()).ToList();

        await _chatHubService.MessageDeleted(recipients, new DeleteChatMessagesDto(chat.Id, deletedMessageIds));
        
        return result.Successful();
    }
    
    public async Task<Result<IEnumerable<ChatDto>>> GetChats(Guid userId, GetChatsRequestDto dto)
    {
        var result = new Result<IEnumerable<ChatDto>>();

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

        var chatsQuery = _dbContext.Entry(realUser).Collection(x => x.UserChatJoins).Query()
            .OrderByDescending(userChat => userChat.Chat.LastMessage != null ? userChat.Chat.LastMessage.SentAt : userChat.Chat.CreatedAt)
            .Select(x => x.ProjectToDto())
            .AsQueryable();
        
        if (dto.Offset > 0)
            chatsQuery = chatsQuery.Skip(dto.Offset);

        if (dto.Count > 0)
            chatsQuery = chatsQuery.Take(dto.Count);

        var chats = await chatsQuery.ToListAsync();

        return result.Successful().WithData(chats);
    }

    public async Task<Result<GetChatMessagesDto>> GetChatMessages(Guid userId, GetChatMessagesRequestDto dto)
    {
        var result = new Result<GetChatMessagesDto>();
        
        var user = await _dbContext.Users
            .Where(user => user.Id == userId)
            .Select(user => new
            {
                Chat = dto.ChatId == null ? null : user.Chats
                    .Where(chat => chat.Id == dto.ChatId)
                    .Select(chat => new
                    {
                        
                    })
                    .FirstOrDefault(),
            })
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (user == null)
            return result.Failed();

        IQueryable<ChatMessage> messagesQueryInit;

        if (dto.ChatId != null)
        {
            if (user.Chat == null)
                return result.Failed().WithError("Chat doesn't exist", "chatNotFound");
            
            var realChat = new UserChat() { Id = dto.ChatId.Value, };

            _dbContext.Attach(realChat);
            
            messagesQueryInit = _dbContext.Entry(realChat).Collection(x => x.ChatMessages).Query()
                .AsNoTracking();
        }
        else
        {
            var realUser = new Application.Entities.IdentityEntities.User() { Id = userId, };

            _dbContext.Attach(realUser);
            
            messagesQueryInit = _dbContext.Entry(realUser).Collection(x => x.Chats).Query()
                .SelectMany(x => x.ChatMessages)
                .AsNoTracking();
        }

        IQueryable<ChatMessage> offsetMessageQuery;

        if (dto.OffsetId is { } offsetId)
        {
            offsetMessageQuery = messagesQueryInit
                .Where(x => x.Id == offsetId);
        }
        else if (dto.OffsetDate is { } offsetDate)
        {
            offsetMessageQuery = messagesQueryInit
                .OrderBy(x => Math.Abs((offsetDate - x.SentAt).TotalMilliseconds));
        }
        else
        {
            throw new Exception("No offset-base was provided! (e.g. OffsetId or OffsetDate)");
        }

        var offset = dto.Offset ?? 0;

        var offsetMessage = await offsetMessageQuery
            .Select(x => new { x.Id, x.SentAt, })
            .FirstOrDefaultAsync();

        if (offsetMessage == null)
            return result.Successful().WithData(new GetChatMessagesDto(new List<ChatMessageDto>()));

        if (dto.SenderId is { } senderId)
        {
            messagesQueryInit = messagesQueryInit
                .Where(x => x.UserId == senderId);
        }

        if (dto.ContainsText is { } containsText)
        {
            var normalizedText = _lookupNormalizer.NormalizeName(containsText);
            
            messagesQueryInit = messagesQueryInit
                .Where(x => x.NormalizedContent.Contains(normalizedText));
        }
        
        var messagesQuery = messagesQueryInit;

        if (offset < 0)
        {
            messagesQuery = messagesQueryInit
                .Where(x => x.SentAt < offsetMessage.SentAt)
                .OrderByDescending(x => x.SentAt)
                .Take(-offset)
                .Concat(messagesQueryInit
                    .Where(x => x.SentAt >= offsetMessage.SentAt)
                )
                .OrderBy(x => x.SentAt);
        }
        else if (offset > 0)
        {
            messagesQuery = messagesQueryInit
                .Where(x => x.SentAt >= offsetMessage.SentAt)
                .OrderBy(x => x.SentAt)
                .Skip(offset);
        }
        else
        {
            messagesQuery = messagesQueryInit
                .Where(x => x.SentAt >= offsetMessage.SentAt)
                .OrderBy(x => x.SentAt);
        }

        messagesQuery = messagesQuery
            .Take(dto.Limit);

        var messages = await messagesQuery
            .Select(x => x.ProjectToDto())
            .ToListAsync();
        
        return result.Successful().WithData(new GetChatMessagesDto(messages));
    }

    public Task<Result<GetChatMessagesDto>> GetPinnedChatMessages(Guid userId, Guid chatId, GetChatMessagesRequestDto dto)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> SetChatUserTypingState(Guid userId, SetUserTypingStateRequestDto dto)
    {
        var result = new Result();
        
        var user = await _dbContext.Users
            .Where(user => user.Id == userId)
            .Select(user => new
            {
                Chat = user.Chats
                    .Where(chat => chat.Id == dto.ChatId)
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

        await _chatHubService.SetUserTypingState(user.Chat.Users.Select(x => x.Id.ToString()), new SetUserTypingStateDto(dto.ChatId, userId, dto.State));

        return result;
    }

    public async Task<Result> PinChatMessage(Guid userId, Guid chatId, Guid messageId)
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
                        
                        Message = chat.ChatMessages
                            .Where(message => message.Id == messageId)
                            .Select(message => new
                            {
                                message.PinnedById,
                            })
                            .FirstOrDefault()
                    })
                    .FirstOrDefault(),
            })
            .AsNoTracking()
            .FirstOrDefaultAsync();
        
        if (user == null)
            return result.Failed();

        if (user.Chat == null)
            return result.Failed().WithError("Chat doesn't exist", "chatNotFound");

        if (user.Chat.Message == null)
            return result.Failed();

        if (user.Chat.Message.PinnedById != null)
            return result.Failed();

        var realMessage = new ChatMessage() { Id = messageId, };

        _dbContext.Attach(realMessage);

        realMessage.PinnedAt = _timeService.GetCurrentTime();
        realMessage.PinnedById = userId;

        await _dbContext.SaveChangesAsync();

        return result;
    }

    public async Task<Result> UnpinChatMessage(Guid userId, Guid chatId, Guid messageId)
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
                        
                        Message = chat.ChatMessages
                            .Where(message => message.Id == messageId)
                            .Select(message => new
                            {
                                message.PinnedById,
                            })
                            .FirstOrDefault()
                    })
                    .FirstOrDefault(),
            })
            .FirstOrDefaultAsync();
        
        if (user == null)
            return result.Failed();

        if (user.Chat == null)
            return result.Failed().WithError("Chat doesn't exist", "chatNotFound");

        if (user.Chat.Message == null)
            return result.Failed();

        if (user.Chat.Message.PinnedById == null)
            return result.Failed();
        
        var realMessage = new ChatMessage() { Id = messageId, };

        _dbContext.Attach(realMessage);

        realMessage.PinnedAt = null;
        realMessage.PinnedById = null;

        await _dbContext.SaveChangesAsync();

        return result;
    }

    public async Task<Result> ChatRead(Guid userId, ChatReadRequestDto dto)
    {
        var result = new Result();

        var lastReadMessageSentAt = dto.LastReadMessageSentAt;

        if (lastReadMessageSentAt.Microsecond == 0)
        {
            lastReadMessageSentAt = lastReadMessageSentAt.AddMicroseconds(999);
        }
        
        var user = await _dbContext.Users
            .Where(user => user.Id == userId)
            .Select(user => new
            {
                UserChat = user.UserChatJoins
                    .Where(userChat => userChat.ChatId == dto.ChatId)
                    .Select(userChat => new
                    {
                        Chat = new
                        {
                            Messages = new
                            {
                                MessagesToRead = userChat.Chat.ChatMessages
                                    .Where(message => message.SentAt > userChat.LastReadMessageSentAt && message.SentAt < lastReadMessageSentAt)
                                    .Select(message => new
                                    {
                                        message.Id,
                                        message.SentAt,
                                        message.ReadAt,
                                        message.ReadCounter
                                    })
                                    .ToList(),
                            
                                UnreadMessagesAmount = userChat.Chat.ChatMessages
                                    .Where(message => message.SentAt > lastReadMessageSentAt)
                                    .Count(),
                            },
                        },
                        
                        userChat.LastReadMessageSentAt,
                    })
                    .FirstOrDefault(),
            })
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (user == null)
            return result.Failed();

        var chat = user.UserChat?.Chat;

        if (chat == null)
            return result.Failed();
        
        if (chat.Messages.MessagesToRead.Count == 0)
            return result.Successful();

        lastReadMessageSentAt = chat.Messages.MessagesToRead[^1].SentAt;

        var realUserChat = new UserChatJoin() { UserId = userId, ChatId = dto.ChatId, };

        _dbContext.Attach(realUserChat);

        realUserChat.LastReadMessageSentAt = lastReadMessageSentAt;

        var currentTime = _timeService.GetCurrentTime();

        foreach (var x in chat.Messages.MessagesToRead)
        {
            var realMessage = new ChatMessage() { Id = x.Id, };

            _dbContext.Attach(realMessage);
            
            if (x.ReadAt == null)
            {
                realMessage.ReadAt = currentTime;
            }

            realMessage.ReadCounter = x.ReadCounter + 1;
        }

        await _dbContext.SaveChangesAsync();
        
        await _chatHubService.ChatRead(userId.ToString(),
            new ChatReadDto(dto.ChatId, lastReadMessageSentAt, chat.Messages.UnreadMessagesAmount));

        return result;
    }

    public async Task<Result<CreateChatDto>> CreateChat(Guid userId, CreateChatRequestDto dto)
    {
        var result = new Result<CreateChatDto>();

        foreach (var memberId in dto.MemberIds)
        {
            if (memberId == userId)
                return result.Failed().WithError("failed", "");
        }

        if (dto.ChatType == ChatType.User)
        {
            var memberId = dto.MemberIds.First();

            var user = await _dbContext.Users.Where(x => x.Id == userId)
                .Select(user => new
                {
                    UserProfileDto = user.UserProfile!.ProjectToDto(),

                    ExistingChat = user.Chats
                        .OfType<UserChat>()
                        .Where(chat => chat.UserFirstId == memberId || chat.UserSecondId == memberId)
                        .Select(chat => new
                        {
                            chat.Id,
                        })
                        .FirstOrDefault(),
                })
                .FirstOrDefaultAsync();
            
            if (user == null)
                return result.Failed();

            if (user.ExistingChat != null)
                return result.Successful().WithData(new CreateChatDto(user.ExistingChat.Id, false));

            var memberProfileDto = await _dbContext.UserProfiles
                .Where(userProfile => userProfile.Id == memberId)
                .Select(userProfile => userProfile.ProjectToDto())
                .FirstOrDefaultAsync();

            if (memberProfileDto == null)
                return result.Failed();

            var currentTime = _timeService.GetCurrentTime();

            var chat = new UserChat()
            {
                Id = Guid.NewGuid(),
                ChatType = dto.ChatType,
                CreatedAt = currentTime,
                
                UserFirstId = userId,
                UserSecondId = memberId,
                
                UserChatJoins = new List<UserChatJoin>()
                {
                  new UserChatJoin() { UserId = userId, LastReadMessageSentAt = currentTime, },  
                  new UserChatJoin() { UserId = memberId, LastReadMessageSentAt = currentTime, },  
                },
            };

            await _dbContext.Chats.AddAsync(chat);
            
            await _dbContext.SaveChangesAsync();

            var userChatDto = new UserChatDto(chat.Id, chat.CreatedAt, chat.CreatedAt, 0, null, memberProfileDto);

            await _chatHubService.ChatNew(new[] { userId.ToString(), }, new ChatJoinDto(userChatDto));
            await _chatHubService.ChatNew(new[] { memberId.ToString(), }, new ChatJoinDto(userChatDto with { UserProfile = user.UserProfileDto, }));

            return result.Successful().WithData(new CreateChatDto(chat.Id, true));
        }
        else
        {
            
        }

        return result;
    }

    public async Task<Result> ClearChat(Guid userId, ClearChatRequestDto dto)
    {
        var result = new Result();

        var chat = await _dbContext.Chats
            .Where(chat => chat.Id == dto.ChatId)
            .Select(chat => new
            { 
                chat.ChatType,
                
                Users = chat.Users.Select(user => new
                {
                    user.Id,
                }).ToList(),
            })
            .FirstOrDefaultAsync();

        if (chat == null)
            return result.Failed();

        var deleteResult = await _dbContext.ChatMessages.Where(x => x.ChatId == dto.ChatId).ExecuteDeleteAsync();

        if (deleteResult > 0)
        {
            await _chatHubService.ChatCleared(chat.Users.Select(user => user.Id.ToString()), new ClearChatDto(dto.ChatId, deleteResult));
        }

        return result.Successful();
    }

    public async Task<Result> DeleteChat(Guid userId, DeleteChatRequestDto dto)
    {
        var result = new Result();

        var chat = await _dbContext.Chats
            .Where(chat => chat.Id == dto.ChatId)
            .Select(chat => new
            { 
                chat.ChatType,
                
                Users = chat.Users.Select(user => new
                {
                    user.Id,
                }).ToList(),
            })
            .FirstOrDefaultAsync();

        if (chat == null)
            return result.Failed();
        
        var deleteResult = await _dbContext.Chats.Where(chat => chat.Id == dto.ChatId).ExecuteDeleteAsync();

        if (deleteResult > 0)
        {
            await _chatHubService.ChatDeleted(chat.Users.Select(user => user.Id.ToString()), new DeleteChatDto(dto.ChatId));
        }

        return result.Successful();
    }
}