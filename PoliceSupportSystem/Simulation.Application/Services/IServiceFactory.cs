using Simulation.Application.Entities;
using Simulation.Shared.Common;

namespace Simulation.Application.Services;

public interface IServiceFactory
{
    IService CreateService(string id, ServiceTypeEnum serviceType) => new Service(id, serviceType);
}