using Shared.Domain.Geo;
using Shared.Domain.Incident;
using Simulation.Application.Entities;

namespace Simulation.Application.Services;

public class EntityFactory : IEntityFactory
{
    public SimulationIncident CreateIncident(
        double latitude,
        double longitude,
        IncidentTypeEnum incidentType = IncidentTypeEnum.NormalIncident,
        IncidentStatusEnum status = IncidentStatusEnum.WaitingForResponse) => CreateIncident(new Position(latitude, longitude), incidentType, status);

    public SimulationIncident CreateIncident(
        Position location,
        IncidentTypeEnum incidentType = IncidentTypeEnum.NormalIncident,
        IncidentStatusEnum status = IncidentStatusEnum.WaitingForResponse) =>
        new(Guid.NewGuid(), location, status, incidentType);
}