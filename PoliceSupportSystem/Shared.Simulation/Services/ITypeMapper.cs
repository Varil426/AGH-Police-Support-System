using Simulation.Communication.Common;

namespace Shared.Simulation.Services;

internal interface ITypeMapper
{
    ServiceTypeEnum Map(Application.Common.ServiceTypeEnum value);
}