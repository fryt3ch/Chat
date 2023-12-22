using FluentValidation;
using Microsoft.AspNetCore.SignalR;

namespace Chat.WebAPI.Middleware;

public class HubValidationFilter : IHubFilter
{
    public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
    {
        foreach (var arg in invocationContext.HubMethodArguments)
        {
            if (arg == null)
                continue;
            
            var validatorType = typeof(IValidator<>).MakeGenericType(arg.GetType());

            if (invocationContext.ServiceProvider.GetService(validatorType) is IValidator validator)
            {
                var res = await validator.ValidateAsync(new ValidationContext<object>(arg));

                if (!res.IsValid)
                {
                    throw new ValidationException(res.Errors);
                }
            }
        }
        
        return await next(invocationContext);
    }
}