using Chat.Application.Dto.UserProfile;
using Chat.Application.Entities.IdentityEntities;
using Chat.Application.Entities.UserProfileEntities;
using Chat.Application.Enums;
using EntityFrameworkCore.Projectables;
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

public static class UserProfileProjectables
{
    [Projectable]
    public static UserProfileDto ProjectToDto(this UserProfile userProfile) => new UserProfileDto(
        userProfile.Id,
        userProfile.User.Username,
        userProfile.Name,
        userProfile.Surname,
        userProfile.Gender,
        userProfile.Country,
        userProfile.BirthDate,
        userProfile.AvatarPhotoId,
        userProfile.Color
    );
}