using System.Diagnostics;
using HqService.Application.Handlers;
using MessageBus.Core.API;
using Shared.Application.Integration.Queries;
using Shared.Infrastructure;
using Shared.Infrastructure.Settings;

namespace HqService.Service;

// internal static class Routing
// {
//     public static void SubscribeQueryHandlers(IAsyncSubscriber subscriber, IServiceProvider serviceProvider) =>
//         subscriber.AddHandler<TestQueryHandler, TestQuery, String>(serviceProvider); // TODO Move to infrastructure
// }