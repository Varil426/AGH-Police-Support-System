using Shared.CommonTypes.Geo;
using Shared.CommonTypes.Incident;
using Simulation.Application.Entities;

namespace Simulation.Application.Services;

internal class EntityFactory : ISimulationIncidentFactory
{
    public SimulationIncident CreateIncident(
        double latitude,
        double longitude,
        IncidentTypeEnum incidentType = IncidentTypeEnum.NormalIncident,
        IncidentStatusEnum status = IncidentStatusEnum.WaitingForResponse,
        DateTimeOffset? createdAt = null) => CreateIncident(new Position(latitude, longitude), incidentType, status, createdAt);
    
    public SimulationIncident CreateIncident(
        Guid incidentId,
        double latitude,
        double longitude,
        IncidentTypeEnum incidentType = IncidentTypeEnum.NormalIncident,
        IncidentStatusEnum status = IncidentStatusEnum.WaitingForResponse,
        DateTimeOffset? createdAt = null) => CreateIncident(incidentId, new Position(latitude, longitude), incidentType, status, createdAt);

    public SimulationIncident CreateIncident(
        Position location,
        IncidentTypeEnum incidentType = IncidentTypeEnum.NormalIncident,
        IncidentStatusEnum status = IncidentStatusEnum.WaitingForResponse,
        DateTimeOffset? createdAt = null) => CreateIncident(Guid.NewGuid(), location, incidentType, status, createdAt);

    public SimulationIncident CreateIncident(
        Guid incidentId,
        Position location,
        IncidentTypeEnum incidentType = IncidentTypeEnum.NormalIncident,
        IncidentStatusEnum status = IncidentStatusEnum.WaitingForResponse,
        DateTimeOffset? createdAt = null) => new(incidentId, location, status, incidentType) { CreatedAt = createdAt ?? DateTimeOffset.UtcNow };
}