using Chat.Domain.Common;
using Chat.Domain.Enums;

namespace Chat.Application.Entities;

public class UserProfile : IEntity<Guid>
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public string Surname { get; set; }
    
    public DateTime BirthDate { get; set; }
    
    public Country Country { get; set; }
}