using Chat.Application.Interfaces;

namespace Chat.Infrastructure.Services;

public class TimeService : ITimeService
{
    public DateTime GetCurrentTime()
    {
        return DateTime.UtcNow;
    }
}