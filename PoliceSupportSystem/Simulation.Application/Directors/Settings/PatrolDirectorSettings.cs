namespace Simulation.Application.Directors.Settings;

/// <summary>
/// Patrol Director Settings.
/// </summary>
/// <param name="NormalPatrolSpeed">In [km/h].</param>
/// <param name="EmergencyPatrolSpeed">In [km/h].</param>
public record PatrolDirectorSettings(double NormalPatrolSpeed, double EmergencyPatrolSpeed)
{
    public double NormalPatrolSpeedMetersPerSecond { get; } = NormalPatrolSpeed * 1000d / (60 * 60);

    public double EmergencyPatrolSpeedMetersPerSecond { get; } = EmergencyPatrolSpeed * 1000d / (60 * 60);
}