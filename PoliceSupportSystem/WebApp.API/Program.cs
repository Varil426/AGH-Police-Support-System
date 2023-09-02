using Shared.Infrastructure;
using WebApp.API;
using WebApp.Application;
using WebApp.Infrastructure;

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
    .RegisterModule<InfrastructureModule>()
    .ConfigureServices(
        s =>
        {
            s.AddMessageService();
            s.AddMessageBus();
            s.AddSignalR();
        });

var app = builder.Build();

app.SubscribeHandlers(new[] { typeof(ApplicationModule).Assembly }); // TODO
app.AddRouting();

app.Run();