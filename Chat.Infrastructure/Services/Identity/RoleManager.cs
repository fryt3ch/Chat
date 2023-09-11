using System.Security.Claims;
using Chat.Application.Entities.Identity;
using Chat.Application.Interfaces.Identity;
using Chat.Domain.Common.Results;
using Chat.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Chat.Infrastructure.Services.Identity;

public class RoleManager : IRoleManager
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILookupNormalizer _lookupNormalizer;

    public RoleManager(ApplicationDbContext dbContext, ILookupNormalizer lookupNormalizer)
    {
        _dbContext = dbContext;
        _lookupNormalizer = lookupNormalizer;
    }

    public async Task<Result> CreateAsync(Role role)
    {
        var result = new Result();
        
        role.NormalizedName = _lookupNormalizer.NormalizeName(role.Name);

        var existingRole = await _dbContext.Roles
            .Where(x => x.NormalizedName == role.NormalizedName)
            .FirstOrDefaultAsync();

        if (existingRole != null)
            return result.Failed().WithError("Role with such name already exists!", "nameExists");

        await _dbContext.Roles.AddAsync(role);

        await _dbContext.SaveChangesAsync();

        return result.Successful();
    }

    public async Task<Result> UpdateAsync(Role role)
    {
        var result = new Result();

        var existingRole = await _dbContext.Roles.FindAsync(role.Id);

        if (existingRole == null)
            return result.Failed().WithError("Role doesn't exist!");
        
        _dbContext.Attach(role);

        role.ConcurrencyStamp = Guid.NewGuid();

        _dbContext.Update(role);
        
        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException e)
        {
            return result.Failed().WithException(e);
        }

        return result.Successful();
    }

    public async Task<Result> DeleteAsync(Role role)
    {
        var result = new Result();

        _dbContext.Remove(role);

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException e)
        {
            return result.Failed().WithException(e);
        }

        return result.Successful();
    }

    public async Task<IList<Claim>> GetClaimsAsync(Role role)
    {
        var result = await _dbContext.RoleClaims
            .Where(x => x.RoleId == role.Id)
            .Select(x => x.ToClaim())
            .ToListAsync();

        return result;
    }
    
    public async Task<Result> AddClaimAsync(Role role, Claim claim)
    {
        var roleClaim = new RoleClaim() { RoleId = role.Id, };
        
        roleClaim.InitializeFromClaim(claim);
        
        _dbContext.RoleClaims.Add(roleClaim);

        return await UpdateAsync(role);
    }
    
    public async Task<Result> RemoveClaimAsync(Role role, Claim claim)
    {
        var claims = await _dbContext.RoleClaims
            .Where(x => x.RoleId == role.Id && x.Type == claim.Type && x.Value == claim.Value)
            .ToListAsync();
        
        _dbContext.RoleClaims.RemoveRange(claims);

        return await UpdateAsync(role);
    }

    public async Task<Role?> FindByNameAsync(string name)
    {
        var normalizedName = _lookupNormalizer.NormalizeName(name);

        var result = await _dbContext.Roles.Where(x => x.NormalizedName == normalizedName).FirstOrDefaultAsync();

        return result;
    }
}