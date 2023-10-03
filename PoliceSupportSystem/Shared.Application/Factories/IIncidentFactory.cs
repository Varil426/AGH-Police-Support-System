using Shared.Application.Integration.DTOs;
using Shared.Domain.Incident;

namespace Shared.Application.Factories;

public interface IIncidentFactory
{
    public Incident CreateIncident(NewIncidentDto newIncidentDto);
    // Incident CreateIncident(IncidentCreatedEvent @event) => CreateIncident(new NewIncidentDto(@event.IncidentId, @event.Location, @event.Type, @event.Status)); // TODO Remove
}