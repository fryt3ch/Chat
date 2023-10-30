using System.Security.Claims;
using Chat.Application.Constants.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Infrastructure.Providers;

public class UserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User.FindFirstValue(IdentityConstants.UserIdClaimType);
    }
}