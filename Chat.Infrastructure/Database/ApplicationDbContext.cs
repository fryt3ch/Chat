using System.Reflection;
using Chat.Application.Entities;
using Chat.Application.Entities.ChatEntities;
using Chat.Application.Entities.IdentityEntities;
using Chat.Application.Entities.UserProfileEntities;
using Chat.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Chat.Infrastructure.Database;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<RoleClaim> RoleClaims { get; set; } = null!;
    public DbSet<UserRole> UserRoles { get; set; } = null!;
    public DbSet<UserClaim> UserClaims { get; set; } = null!;
    
    public DbSet<UserProfile> UserProfiles { get; set; } = null!;

    public DbSet<ChatMessage> ChatMessages { get; set; } = null!;
    public DbSet<Application.Entities.ChatEntities.Chat> Chats { get; set; } = null!;
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}