using System.Collections;

namespace Simulation.Application.Helpers;

internal static class ExceptionExtension
{
    static async Task<T> ThrowIfNotFound<T>(this Task<T> task)
    {
        var result = await task;
        switch (result)
        {
            case IEnumerable enumerable:
                if (!enumerable.GetEnumerator().MoveNext())
                    throw new Exception("Not found");
                break;
            case null:
                throw new Exception("Not found");
        }

        return result;
    }
}