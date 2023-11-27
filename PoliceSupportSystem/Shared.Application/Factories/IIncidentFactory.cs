using Shared.Application.Integration.DTOs;
using Shared.Domain.Incident;

namespace Shared.Application.Factories;

public interface IIncidentFactory
{
    public Incident CreateIncident(NewIncidentDto newIncidentDto);
}