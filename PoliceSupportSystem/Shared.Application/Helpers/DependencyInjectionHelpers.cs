using Autofac;
using Autofac.Builder;
using Microsoft.Extensions.Hosting;

namespace Shared.Application.Helpers;

public static class DependencyInjectionHelpers
{
    public static IRegistrationBuilder<TType, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterHostedService<TType>(this ContainerBuilder builder) where TType : IHostedService 
        => builder.RegisterType<TType>().As<IHostedService>().AsSelf().SingleInstance();
}