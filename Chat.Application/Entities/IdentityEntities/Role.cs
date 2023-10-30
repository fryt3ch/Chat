using Chat.Domain.Common;

namespace Chat.Application.Entities.IdentityEntities;

public class Role : IEntity<Guid>
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public string NormalizedName { get; set; }

    public Guid ConcurrencyStamp { get; set; } = Guid.NewGuid();
}