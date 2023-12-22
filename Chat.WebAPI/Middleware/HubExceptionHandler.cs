using Microsoft.AspNetCore.SignalR;

namespace Chat.WebAPI.Middleware;

public class HubExceptionHandler : IHubFilter
{
    public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
    {
        try
        {
            return await next(invocationContext);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            
            throw;
        }
    }
}