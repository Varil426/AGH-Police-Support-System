namespace Simulation.Application.Directors;

public interface IDirector
{
    Task InvokeAsync(ISimulation simulation);
}