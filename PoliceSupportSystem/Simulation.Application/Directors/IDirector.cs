namespace Simulation.Application.Directors;

public interface IDirector
{
    Task Act(ISimulation simulation);
}