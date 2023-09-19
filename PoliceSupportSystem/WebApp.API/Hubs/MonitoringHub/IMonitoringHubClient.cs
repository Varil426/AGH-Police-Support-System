using Reinforced.Typings.Attributes;

namespace WebApp.API.Hubs.MonitoringHub;

[TsInterface]
public interface IMonitoringHubClient
{
    Task ReceiveUpdate(CityStateMessageDto cityStateMessageDto);
}