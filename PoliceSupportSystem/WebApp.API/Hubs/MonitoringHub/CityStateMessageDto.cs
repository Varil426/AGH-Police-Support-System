using Reinforced.Typings.Attributes;
using Shared.Application.Integration.DTOs;
using Shared.CommonTypes.Geo;

namespace WebApp.API.Hubs.MonitoringHub;

[TsInterface]
public record CityStateMessageDto(Position HqLocation, IEnumerable<IncidentDto> Incidents, IEnumerable<PatrolDto> Patrols);