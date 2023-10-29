using Shared.CommonTypes.Geo;
using Shared.CommonTypes.Incident;
using Simulation.Application.Entities;
using Simulation.Application.Entities.Incident;

namespace Simulation.Application.Services;

public interface ISimulationIncidentFactory
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