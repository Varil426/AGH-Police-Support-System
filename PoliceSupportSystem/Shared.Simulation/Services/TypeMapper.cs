using Simulation.Shared.Common;

namespace Shared.Simulation.Services;

internal class TypeMapper : ITypeMapper
{
    public ServiceTypeEnum Map(Application.Common.ServiceTypeEnum value) => Enum.Parse<ServiceTypeEnum>(value.ToString());
}