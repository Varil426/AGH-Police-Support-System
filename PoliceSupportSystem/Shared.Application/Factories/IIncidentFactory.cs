using Shared.Application.DTOs;
using Shared.Domain.Incident;

namespace Shared.Application.Factories;

public interface IIncidentFactory
{
    Incident CreateIncident(NewIncidentDto newIncidentDto);
}