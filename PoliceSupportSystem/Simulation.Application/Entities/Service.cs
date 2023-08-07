using Simulation.Shared.Common;

namespace Simulation.Application.Entities;

public record Service(string Id, ServiceTypeEnum ServiceType) : IService;