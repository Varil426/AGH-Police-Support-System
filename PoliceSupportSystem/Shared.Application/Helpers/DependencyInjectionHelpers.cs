using Autofac;
using Autofac.Builder;
using Microsoft.Extensions.Hosting;
using Shared.Application.Agents;

namespace Shared.Application.Helpers;

public static class DependencyInjectionHelpers
{
    public static IRegistrationBuilder<TType, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterAgent<TType>(this ContainerBuilder builder) where TType : IAgent
        => builder.RegisterHostedService<TType>().AsSelf();
    
    public static IRegistrationBuilder<TType, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterHostedService<TType>(this ContainerBuilder builder) where TType : IHostedService 
        => builder.RegisterType<TType>().As<IHostedService>().AsSelf().SingleInstance();
}