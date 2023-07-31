using Shared.Application;
using Shared.Application.Handlers;
using Shared.Application.Integration.Queries;

namespace HqService.Application.Handlers;

public class TestQueryHandler : IQueryHandler<TestQuery, string>
{
    public Task<string> Handle(TestQuery query)
    {
        Console.WriteLine("Success");
        return Task.FromResult($"{query.Value * 2}");
    }
}