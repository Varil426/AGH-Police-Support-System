using Shared.Domain.Incident;

namespace PatrolService.Application.Services;

public interface IConfirmationService
{
    public Task ConfirmIncidentStart(Guid incidentId);
    public Task ConfirmSupportShooting(Guid incidentId);
}