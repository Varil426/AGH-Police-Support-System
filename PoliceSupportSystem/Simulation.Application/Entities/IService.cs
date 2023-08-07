using Simulation.Shared.Common;

namespace Simulation.Application.Entities;

public interface IService
{
    string Id { get; }
    
    ServiceTypeEnum ServiceType { get; }
}