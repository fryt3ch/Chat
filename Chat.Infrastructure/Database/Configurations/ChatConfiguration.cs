using Chat.Application.Entities;
using Chat.Application.Entities.ChatEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chat.Infrastructure.Database.Configurations;

public class ChatConfiguration : IEntityTypeConfiguration<Application.Entities.ChatEntities.Chat>
{
    public void Configure(EntityTypeBuilder<Application.Entities.ChatEntities.Chat> builder)
    {
        builder.ToTable("Chats");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.LastMessage)
            .WithMany()
            .HasForeignKey(x => x.LastMessageId)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasMany(x => x.Users)
            .WithMany(x => x.Chats)
            .UsingEntity<UserChat>(
                j => j
                    .HasOne(x => x.User)
                    .WithMany(x => x.UserChats)
                    .HasForeignKey(x => x.UserId),
                j => j
                    .HasOne(x => x.Chat)
                    .WithMany(x => x.UserChats)
                    .HasForeignKey(x => x.ChatId),
                j =>
                {
                    j.ToTable("UserChats");
                    
                    j.HasKey(x => new { x.UserId, x.ChatId });
                });
    }
}