using Chat.Application.Entities;
using Chat.Application.Entities.ChatEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chat.Infrastructure.Database.Configurations;

public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.ToTable("ChatMessages");

        builder.HasOne(x => x.Chat)
            .WithMany(x => x.ChatMessages)
            .HasForeignKey(x => x.ChatId);

        builder.HasOne(x => x.User)
            .WithMany(x => x.ChatMessages)
            .HasForeignKey(x => x.UserId);
    }
}