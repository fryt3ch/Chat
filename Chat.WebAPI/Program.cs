using System.Reflection;
using Chat.Application;
using Chat.Application.Dto.Chat;
using Chat.Application.Interfaces.Hubs;
using Chat.Infrastructure;
using Chat.Infrastructure.Database;
using Chat.WebAPI.Hubs;
using Chat.WebAPI.Middleware;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddCors();

builder.Services
    .AddControllers()
    .AddNewtonsoftJson(options =>
    {
        
    });

builder.Services
    .AddValidatorsFromAssembly(Assembly.Load("Chat.Application"))
    .AddFluentValidationAutoValidation(x =>
    {
        x.DisableDataAnnotationsValidation = true;
    });

builder.Services.AddSignalR(options =>
    {
        options.EnableDetailedErrors = true;
        
        options.AddFilter<HubExceptionHandler>();
        options.AddFilter<HubValidationFilter>();
    })
    .AddNewtonsoftJsonProtocol();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IChatHubService, ChatHubService>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("hubs/chat");

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();

    await initializer.InitializeAsync();

    await initializer.SeedAsync();   
}

app.UseCors(config =>
{
    config
        .AllowCredentials()
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithOrigins("localhost:4200");
});

app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources")),
    RequestPath = new PathString("/Resources")
});

app.Run();