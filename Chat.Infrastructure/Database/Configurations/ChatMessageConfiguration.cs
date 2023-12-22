using Chat.Application.Dto.Chat;
using Chat.Application.Entities;
using Chat.Application.Entities.ChatEntities;
using Chat.Application.Enums;
using EntityFrameworkCore.Projectables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chat.Infrastructure.Database.Configurations;

public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.ToTable("ChatMessages");
        
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.ChatId, x.ChatMessageId, });

        builder.HasOne(x => x.Chat)
            .WithMany(x => x.ChatMessages)
            .HasForeignKey(x => x.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany(x => x.ChatMessages)
            .HasForeignKey(x => x.UserId);

        builder.HasOne(x => x.PinnedBy)
            .WithMany()
            .HasForeignKey(x => x.PinnedById);

        builder.HasOne(x => x.SourceUser)
            .WithMany()
            .IsRequired(false)
            .HasForeignKey(x => x.SourceUserId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasOne(x => x.SourceMessage)
            .WithMany()
            .IsRequired(false)
            .HasForeignKey(x => x.SourceMessageId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public static class ChatMessageProjectables
{
    [Projectable]
    public static ChatMessageDto ProjectToDto(this ChatMessage chatMessage) => new ChatMessageDto(
        chatMessage.Id,
        chatMessage.MessageType,
        chatMessage.ChatId,
        chatMessage.UserId,
        chatMessage.Content,
        chatMessage.SentAt,
        chatMessage.ReceivedAt,
        chatMessage.ReadAt,
        chatMessage.MessageType != ChatMessageType.Quoted || chatMessage.SourceMessage == null
            ? null
            : new ChatMessageDto(
                chatMessage.SourceMessage.Id,
                chatMessage.SourceMessage.MessageType,
                chatMessage.SourceMessage.ChatId,
                chatMessage.SourceMessage.UserId,
                chatMessage.SourceMessage.Content,
                chatMessage.SourceMessage.SentAt,
                null, null, null, null
            ),
        chatMessage.SourceUserId
    );
}