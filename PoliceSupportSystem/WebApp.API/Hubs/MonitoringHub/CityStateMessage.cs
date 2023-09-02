using Reinforced.Typings.Attributes;
using Shared.Application.DTOs;

namespace WebApp.API.Hubs.MonitoringHub;

[TsInterface]
public record CityStateMessage(IEnumerable<IncidentDto> Incidents);