using Chat.Application.Entities.IdentityEntities;
using Chat.Application.Enums;
using Chat.Domain.Common;
using Chat.Domain.Enums;

namespace Chat.Application.Entities.UserProfileEntities;

public class UserProfile : IEntity<Guid>
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public string Surname { get; set; }
    
    public Gender Gender { get; set; }
    
    public DateTime BirthDate { get; set; }
    
    public Country Country { get; set; }
    
    public string? AvatarPhotoId { get; set; }
    
    public User User { get; set; }
    
    public ProfileColor Color { get; set; }
}