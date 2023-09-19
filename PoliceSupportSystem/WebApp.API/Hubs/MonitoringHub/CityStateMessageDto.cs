using Reinforced.Typings.Attributes;
using Shared.Application.Integration.DTOs;

namespace WebApp.API.Hubs.MonitoringHub;

[TsInterface]
public record CityStateMessageDto(IEnumerable<IncidentDto> Incidents);