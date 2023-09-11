using Chat.Application.Constants.Identity;
using Chat.Application.Interfaces;
using Chat.Application.Interfaces.Identity;
using Chat.Infrastructure.Database;
using Chat.Infrastructure.Services;
using Chat.Infrastructure.Services.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chat.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(o =>
        {
            o.UseSqlite(configuration.GetConnectionString("appDatabase"));
        }, ServiceLifetime.Scoped);

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<ApplicationDbContextInitializer>();

        services.AddHttpContextAccessor();

        services
            .AddScoped<IUserManager, UserManager>()
            .AddScoped<IRoleManager, RoleManager>()
            .AddScoped<IUserClaimsPrincipalFactory, UserClaimsPrincipalFactory>()
            .AddScoped<ISignInManager, SignInManager>()
            .AddScoped<IPasswordHasher, PasswordHasher>()
            .AddScoped<ILookupNormalizer, LookupNormalizer>()
            .AddScoped<IUserProfileManager, UserProfileManager>();

        services.AddAuthentication(IdentityConstants.ApplicationScheme)
            .AddCookie(IdentityConstants.ApplicationScheme, o =>
            {
                o.Events.OnRedirectToLogin = (context) =>
                {
                    context.Response.StatusCode = 401;

                    return Task.CompletedTask;
                };

                o.Events.OnRedirectToAccessDenied = (context) =>
                {
                    context.Response.StatusCode = 403;

                    return Task.CompletedTask;
                };
            })
            .AddCookie(IdentityConstants.ExternalScheme);

        services.AddAuthorization(o =>
        {
            o.AddPolicy("AdministratorPolicy", c =>
            {
                c.RequireAuthenticatedUser();
                c.RequireRole("Administrator");
            });
        });

        services.AddSignalR();

        return services;
    }
}