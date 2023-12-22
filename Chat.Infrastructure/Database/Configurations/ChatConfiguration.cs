using System.Linq.Expressions;
using Chat.Application.Dto.Chat;
using Chat.Application.Dto.UserProfile;
using Chat.Application.Entities;
using Chat.Application.Entities.ChatEntities;
using Chat.Application.Entities.IdentityEntities;
using Chat.Application.Entities.UserProfileEntities;
using Chat.Application.Enums;
using EntityFrameworkCore.Projectables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chat.Infrastructure.Database.Configurations;

public class ChatConfiguration : IEntityTypeConfiguration<Application.Entities.ChatEntities.Chat>
{
    public void Configure(EntityTypeBuilder<Application.Entities.ChatEntities.Chat> builder)
    {
        builder.UseTphMappingStrategy();

        builder.HasDiscriminator(x => x.ChatType)
            .HasValue<UserChat>(ChatType.User)
            .HasValue<GroupChat>(ChatType.Group);

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.LastMessage)
            .WithMany()
            .HasForeignKey(x => x.LastMessageId)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasMany(x => x.Users)
            .WithMany(x => x.Chats)
            .UsingEntity<UserChatJoin>(
                l => l
                    .HasOne(x => x.User)
                    .WithMany(x => x.UserChatJoins)
                    .HasForeignKey(x => x.UserId)
                    .HasPrincipalKey(x => x.Id)
                    .OnDelete(DeleteBehavior.Cascade),
                r => r
                    .HasOne(x => x.Chat)
                    .WithMany(x => x.UserChatJoins)
                    .HasForeignKey(x => x.ChatId)
                    .HasPrincipalKey(x => x.Id)
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.ToTable("UserChatJoins");
                    
                    j.HasKey(x => new { x.UserId, x.ChatId });
                });
    }
}

public class UserChatConfiguration : IEntityTypeConfiguration<UserChat>
{
    public void Configure(EntityTypeBuilder<UserChat> builder)
    {
        //builder.ToTable("UserChats");

        builder.HasIndex(x => new { x.UserFirstId, x.UserSecondId }).IsUnique(true);

        /*builder.HasIndex(x => x.UserFirstId).IsUnique(false);
        builder.HasIndex(x => x.UserSecondId).IsUnique(false);*/

        builder.HasOne(x => x.UserFirst)
            .WithOne();
        
        builder.HasOne(x => x.UserSecond)
            .WithOne();
    }
}

public class GroupChatConfiguration : IEntityTypeConfiguration<Application.Entities.ChatEntities.GroupChat>
{
    public void Configure(EntityTypeBuilder<Application.Entities.ChatEntities.GroupChat> builder)
    {
        //builder.ToTable("GroupChats");
    }
}

public static class ChatProjectables
{
    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static ChatDto ProjectToDto(this UserChatJoin userChatJoin) => userChatJoin.Chat.ChatType == ChatType.User ? new UserChatDto(
        userChatJoin.Chat.Id,
        userChatJoin.Chat.CreatedAt,
        userChatJoin.LastReadMessageSentAt,
        userChatJoin.Chat.ChatMessages.Where(message => message.SentAt > userChatJoin.LastReadMessageSentAt).Count(),
        userChatJoin.Chat.LastMessage?.ProjectToDto(),
        
        ((UserChat)userChatJoin.Chat).GetOppositeUserProfileDto(userChatJoin.UserId)
    ) : new GroupChatDto(
        userChatJoin.Chat.Id,
        userChatJoin.Chat.CreatedAt,
        userChatJoin.LastReadMessageSentAt,
        userChatJoin.Chat.ChatMessages.Where(message => message.SentAt > userChatJoin.LastReadMessageSentAt).Count(),
        userChatJoin.Chat.LastMessage?.ProjectToDto(),
        
        ProfileColor.Blue,
        null,
        ""
    );

    [Projectable]
    public static User GetOppositeUser(this UserChat userChat, Guid userId) =>
        userChat.UserFirstId == userId ? userChat.UserSecond : userChat.UserFirst;
    
    [Projectable]
    public static UserProfileDto GetOppositeUserProfileDto(this UserChat userChat, Guid userId) =>
        userChat.UserFirstId == userId ? userChat.UserSecond.UserProfile.ProjectToDto() : userChat.UserFirst.UserProfile.ProjectToDto();
}