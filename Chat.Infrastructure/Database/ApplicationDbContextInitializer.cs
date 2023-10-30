using System.Security.Claims;
using Chat.Application.Entities;
using Chat.Application.Entities.ChatEntities;
using Chat.Application.Entities.IdentityEntities;
using Chat.Application.Interfaces;
using Chat.Application.Interfaces.Identity;
using Chat.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Chat.Infrastructure.Database;

public class ApplicationDbContextInitializer
{
    private readonly ApplicationDbContext _dbContext;

    private readonly IUserManager _userManager;
    private readonly IRoleManager _roleManager;
    private readonly IUserProfileManager _userProfileManager;
    
    private readonly ILogger<ApplicationDbContext> _logger;

    public ApplicationDbContextInitializer(ApplicationDbContext dbContext, ILogger<ApplicationDbContext> logger, IUserManager userManager, IRoleManager roleManager, IUserProfileManager userProfileManager)
    {
        _dbContext = dbContext;
        _logger = logger;
        _userManager = userManager;
        _roleManager = roleManager;
        _userProfileManager = userProfileManager;
    }

    public async Task InitializeAsync()
    {
        try
        {
            if (_dbContext.Database.IsRelational())
            {
                await _dbContext.Database.MigrateAsync();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occured while initialize database!");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            var administrator = new User()
            {
                Username = "frytech",
                Email = "frytech@mail.ru",
                Password = "qwerty123",
            };

            var defaultUser = new User()
            {
                Username = "test-guy",
                Password = "qwerty",
            };

            var administratorRole = new Role()
            {
                Name = "Administrator",
            };

            await _userManager.CreateAsync(defaultUser);
            
            if ((await _userManager.CreateAsync(administrator)).Failed)
                administrator = await _userManager.FindByUsernameAsync(administrator.Username);
            
            /*await _roleManager.CreateAsync(administratorRole);

            await _roleManager.AddClaimAsync(administratorRole, new Claim("deleteUser", "true"));*/

            /*if (administrator != null)
            {
                await _userProfileManager.CreateAsync(new UserProfile()
                {
                    Id = administrator.Id,
                    Name = "Evgeny", Surname = "Semenov",
                    BirthDate = new DateTime(2002, 4, 17),
                    Country = Country.Russia,
                });
            }*/
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occured while seed database!");
            throw;
        }
    }
}