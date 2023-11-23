using Shared.Infrastructure;
using WebApp.API;
using WebApp.API.Handlers;
using WebApp.API.Workers;
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
    .RegisterModule(new ApplicationModule(builder.Configuration))
    .RegisterModule<InfrastructureModule>()
    .ConfigureServices(
        s =>
        {
            s.AddMessageService();
            s.AddMessageBus();
            s.AddSignalR()/*.AddJsonProtocol(options => {
                options.PayloadSerializerOptions.PropertyNamingPolicy = null;
            })*/;
            s.AddHostedService<MonitoringHubEmitter>();

            s.AddCors(
                config =>
                {
                    config.AddDefaultPolicy(
                        x =>
                        {
                            x.WithOrigins("http://localhost:3004")
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials();
                        });
                });

            s.AddScoped<GetStatisticsHandler>();
        });

var app = builder.Build();

app.SubscribeHandlers(new[] { typeof(ApplicationModule).Assembly });
app.AddRouting();
app.UseCors();

app.Run();