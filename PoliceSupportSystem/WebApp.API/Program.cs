using System.Reflection;
using Shared.Infrastructure;
using WebApp.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Host
    .UseAutofac()
    .AddSerilog()
    .AddSharedAppSettings()
    .AddRabbitMqSettings()
    .AddServiceSettings()
    .AddRabbitMqBus(new[] { typeof(ApplicationModule).Assembly }) // TODO
    .RegisterSharedApplicationModule()
    .RegisterModule<ApplicationModule>()
    .ConfigureServices(
        s =>
        {
            s.AddMessageService();
            s.AddMessageBus();
        });

var app = builder.Build();

app.SubscribeHandlers(new[] { typeof(ApplicationModule).Assembly }); // TODO

app.Run();