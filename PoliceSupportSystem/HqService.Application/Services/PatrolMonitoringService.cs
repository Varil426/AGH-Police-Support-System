using Microsoft.Extensions.Logging;
using Shared.Domain.Patrol;

namespace HqService.Application.Services;

internal class PatrolMonitoringService : IPatrolMonitoringService
{
    private readonly ILogger<PatrolMonitoringService> _logger;
    private readonly List<Patrol> _patrols = new();

    public IReadOnlyCollection<Patrol> Patrols => _patrols.AsReadOnly();

    public PatrolMonitoringService(ILogger<PatrolMonitoringService> logger)
    {
        _logger = logger;
    }

    public void AddPatrol(Patrol patrol)
    {
        if (_patrols.Any(x => x.PatrolId.Equals(patrol.PatrolId, StringComparison.InvariantCultureIgnoreCase)))
        {
            _logger.LogWarning("Attempted to add a duplicated patrol (PatrolId: {patrolId})", patrol.PatrolId);
            return;
        }
        _patrols.Add(patrol);
    }

    public void RemovePatrol(Guid id)
    {
        var patrol = _patrols.FirstOrDefault(x => x.Id == id);
        if (patrol is not null)
            _patrols.Remove(patrol);
        else
        {
            _logger.LogWarning("Attempted to remove not existing patrol (Id: {Id})", id);
        }
    }

    public void RemovePatrol(string patrolId)
    {
        var patrol = _patrols.FirstOrDefault(x => x.PatrolId.Equals(patrolId, StringComparison.InvariantCultureIgnoreCase));
        if (patrol is not null)
            _patrols.Remove(patrol);
        else
        {
            _logger.LogWarning("Attempted to remove not existing patrol (PatrolId: {patrolId})", patrolId);
        }
    }
}