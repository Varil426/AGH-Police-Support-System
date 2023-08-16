using Shared.Domain.Geo;
using Shared.Domain.Incident;
using Simulation.Application.Entities;

namespace Simulation.Application.Services;

public interface IEntityFactory
{
    SimulationIncident CreateIncident(
        double latitude,
        double longitude,
        IncidentTypeEnum incidentType = IncidentTypeEnum.NormalIncident,
        IncidentStatusEnum status = IncidentStatusEnum.WaitingForResponse,
        DateTimeOffset? cratedAt = null);

    SimulationIncident CreateIncident(
        Position location,
        IncidentTypeEnum incidentType = IncidentTypeEnum.NormalIncident,
        IncidentStatusEnum status = IncidentStatusEnum.WaitingForResponse,
        DateTimeOffset? cratedAt = null);

    SimulationIncident CreateIncident(
        Guid incidentId,
        Position location,
        IncidentTypeEnum incidentType = IncidentTypeEnum.NormalIncident,
        IncidentStatusEnum status = IncidentStatusEnum.WaitingForResponse,
        DateTimeOffset? cratedAt = null);

    SimulationIncident CreateIncident(
        Guid incidentId,
        double latitude,
        double longitude,
        IncidentTypeEnum incidentType = IncidentTypeEnum.NormalIncident,
        IncidentStatusEnum status = IncidentStatusEnum.WaitingForResponse,
        DateTimeOffset? cratedAt = null);
}