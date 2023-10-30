using System.Security.Claims;
using Chat.Application.Entities.IdentityEntities;
using Chat.Application.Interfaces.Identity;
using Chat.Domain.Common.Results;
using Chat.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Chat.Infrastructure.Services.Identity;

public class UserManager : IUserManager
{
    private readonly ApplicationDbContext _dbContext;

    public readonly IPasswordHasher PasswordHasher;
    public readonly ILookupNormalizer LookupNormalizer;

    public UserManager(ApplicationDbContext dbContext, IPasswordHasher passwordHasher, ILookupNormalizer lookupNormalizer)
    {
        _dbContext = dbContext;
        
        PasswordHasher = passwordHasher;
        LookupNormalizer = lookupNormalizer;
    }

    public async Task<Result> CreateAsync(User user)
    {
        var result = new Result();
        
        user.Id = Guid.NewGuid();

        user.Password = PasswordHasher.Hash(user.Password);

        user.NormalizedUsername = LookupNormalizer.NormalizeName(user.Username);

        IQueryable<User> existingLookupQuery = _dbContext.Users;

        if (user.Email != null)
        {
            user.NormalizedEmail = LookupNormalizer.NormalizeEmail(user.Email);

            if (user.PhoneNumber != null)
            {
                existingLookupQuery = existingLookupQuery
                    .Where(x => x.NormalizedUsername == user.NormalizedUsername || x.NormalizedEmail == user.NormalizedEmail || x.PhoneNumber == user.PhoneNumber);
            }
            else
            {
                existingLookupQuery = existingLookupQuery
                    .Where(x => x.NormalizedUsername == user.NormalizedUsername || x.NormalizedEmail == user.NormalizedEmail);
            }
        }
        else if (user.PhoneNumber != null)
        {
            existingLookupQuery = existingLookupQuery
                .Where(x => x.NormalizedUsername == user.NormalizedUsername || x.PhoneNumber == user.PhoneNumber);
        }
        else
        {
            existingLookupQuery = existingLookupQuery
                .Where(x => x.NormalizedUsername == user.NormalizedUsername);
        }
        
        var existingUser = await existingLookupQuery
            .Select(x => new { x.NormalizedUsername, x.NormalizedEmail, x.PhoneNumber, })
            .FirstOrDefaultAsync();

        if (existingUser != null)
        {
            if (user.NormalizedUsername == existingUser.NormalizedUsername)
                return result.Failed().WithError("User with such username already exists!", "usernameExists");
            
            if (user.NormalizedEmail != null && user.NormalizedEmail == existingUser.NormalizedEmail)
                return result.Failed().WithError("User with such e-mail already exists!", "emailExists");
            
            if (user.PhoneNumber != null && user.PhoneNumber == existingUser.PhoneNumber)
                return result.Failed().WithError("User with such phone number already exists!", "phoneNumberExists");

            return result.Failed();
        }
        
        await _dbContext.Users.AddAsync(user);

        await _dbContext.SaveChangesAsync();

        return result.Successful();
    }

    public async Task<Result> UpdateAsync(User user)
    {
        var result = new Result();

        var existingUser = await _dbContext.Users.FindAsync(user.Id);

        if (existingUser == null)
            return result.Failed().WithError("User doesn't exist!", "userNotFound");

        _dbContext.Attach(user);

        user.ConcurrencyStamp = Guid.NewGuid();

        _dbContext.Update(user);
        
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

    public async Task<Result> DeleteAsync(User user)
    {
        var result = new Result();
        
        _dbContext.Users.Remove(user);

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
    
    public async Task<Result> CheckPasswordAsync(User user, string password)
    {
        var result = PasswordHasher.Verify(password, user.Password);

        return result;
    }

    public async Task<User?> FindByUsernameAsync(string username)
    {
        var normalizedUsername = LookupNormalizer.NormalizeName(username);

        return await _dbContext.Users.FirstOrDefaultAsync(x => x.NormalizedUsername == normalizedUsername);
    }

    public async Task<IList<string>> GetRolesAsync(User user)
    {
        var result = await _dbContext.UserRoles
            .Join(_dbContext.Roles, userRole => userRole.RoleId, role => role.Id,
                (userRole, role) => new { UserRole = userRole, Role = role, })
            .Where(x => x.UserRole.UserId == user.Id)
            .Select(x => x.Role.Name)
            .ToListAsync();

        return result;
    }

    public async Task<bool> IsInRoleAsync(User user, string normalizedRoleName)
    {
        var role = await _dbContext.Roles.Where(x => x.NormalizedName == normalizedRoleName).FirstOrDefaultAsync();

        if (role == null)
            return false;

        var userRole = await _dbContext.UserRoles.FindAsync(new object[] { user.Id, role.Id });

        return userRole != null;
    }

    public async Task<Result> AddToRoleAsync(User user, string normalizedRoleName)
    {
        var result = new Result();

        if (await IsInRoleAsync(user, normalizedRoleName))
            return result.Successful().WithError("User is already in this role, no action needed!", "alreadyInRole");
        
        var role = await _dbContext.Roles.Where(x => x.NormalizedName == normalizedRoleName).FirstOrDefaultAsync();

        if (role == null)
            return result.Failed().WithError("Role doesn't exist!", "roleNotFound");

        var userRole = new UserRole() { RoleId = role.Id, UserId = user.Id, };

        await _dbContext.UserRoles.AddAsync(userRole);

        return await UpdateAsync(user);
    }
    
    public async Task<Result> RemoveFromRoleAsync(User user, string normalizedRoleName)
    {
        var result = new Result();

        if (!await IsInRoleAsync(user, normalizedRoleName))
            return result.Successful().WithError("User is already not in this role, no action needed!", "alreadyNotInRole");
        
        var role = await _dbContext.Roles.Where(x => x.NormalizedName == normalizedRoleName).FirstOrDefaultAsync();

        if (role == null)
            return result.Successful();

        var userRole = (await _dbContext.UserRoles.FindAsync(new object[] { user.Id, role.Id, }));

        if (userRole == null)
            return result.Successful();

        _dbContext.UserRoles.Remove(userRole);

        return await UpdateAsync(user);
    }

    public async Task<IList<Claim>> GetClaimsAsync(User user)
    {
        return await _dbContext.UserClaims.Where(x => x.UserId == user.Id).Select(x => x.ToClaim()).ToListAsync();
    }

    public int Test()
    {
        return _dbContext.GetHashCode();
    }
}