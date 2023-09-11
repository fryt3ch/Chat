using Chat.Application.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chat.Infrastructure.Database.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.NormalizedName)
            .HasDatabaseName("RoleNameIdx")
            .IsUnique();

        builder.HasMany<UserRole>()
            .WithOne()
            .HasForeignKey(x => x.RoleId)
            .HasPrincipalKey(x => x.Id)
            .IsRequired();
        
        builder.HasMany<RoleClaim>()
            .WithOne()
            .HasForeignKey(x => x.RoleId)
            .HasPrincipalKey(x => x.Id)
            .IsRequired();
        
        builder.Property(x => x.ConcurrencyStamp)
            .IsConcurrencyToken(true);
    }
}