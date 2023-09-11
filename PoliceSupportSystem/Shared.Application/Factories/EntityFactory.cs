using Shared.Application.Integration.DTOs;
using Shared.Domain.Incident;

namespace Shared.Application.Factories;

internal class EntityFactory : IIncidentFactory
{
    public Incident CreateIncident(NewIncidentDto newIncidentDto) => new(newIncidentDto.Id, newIncidentDto.Location, newIncidentDto.Status, newIncidentDto.Type);
}