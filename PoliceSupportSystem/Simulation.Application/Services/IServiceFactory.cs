using Simulation.Application.Entities;
using Simulation.Communication.Common;

namespace Simulation.Application.Services;

public interface IServiceFactory
{
    IService CreateService(string id, ServiceTypeEnum serviceType) => new Service(id, serviceType);
}