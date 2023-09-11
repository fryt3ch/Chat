using Chat.Application.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chat.Infrastructure.Database.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.NormalizedUsername)
            .HasDatabaseName("UsernameIdx")
            .IsUnique();

        builder.HasIndex(x => x.NormalizedEmail)
            .HasDatabaseName("EmailIdx");

        builder.Property(x => x.ConcurrencyStamp)
            .IsConcurrencyToken(true);
    }
}