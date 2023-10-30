using Chat.Application.Entities;
using Chat.Application.Entities.IdentityEntities;
using Chat.Application.Entities.UserProfileEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chat.Infrastructure.Database.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("UserProfiles");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.User)
            .WithOne(x => x.UserProfile)
            .HasPrincipalKey<User>(x => x.Id)
            .HasForeignKey<UserProfile>(x => x.Id);
    }
}