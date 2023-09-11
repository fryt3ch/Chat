using Chat.Application.Entities;
using Chat.Application.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chat.Infrastructure.Database.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("UserProfiles");

        builder.HasKey(x => x.Id);

        builder.HasOne<User>()
            .WithOne()
            .HasPrincipalKey<User>(x => x.Id)
            .HasForeignKey<UserProfile>(x => x.Id);
    }
}