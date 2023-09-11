namespace Chat.Application.Interfaces.Hubs;

public interface IChatHubClient
{
    Task SendPrivate();
}