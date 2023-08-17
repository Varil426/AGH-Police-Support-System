using System.Reflection;
using Shared.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Host
    .UseAutofac()
    .AddSerilog()
    .AddSharedAppSettings()
    .AddRabbitMqSettings()
    .AddServiceSettings()
    .AddRabbitMqBus(new Assembly[] { }) // TODO
    .ConfigureServices(
        s =>
        {
            s.AddMessageService();
            s.AddMessageBus();
        });

var app = builder.Build();

app.SubscribeHandlers(new Assembly[] { }); // TODO

app.Run();