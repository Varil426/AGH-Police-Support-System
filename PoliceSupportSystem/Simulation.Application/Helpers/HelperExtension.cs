namespace Simulation.Application.Helpers;

public static class HelperExtension
{
    public static TV? TryGet<TK, TV>(this IDictionary<TK, TV> dictionary, TK key)
    {
        dictionary.TryGetValue(key, out var result);
        return result;
    }
    public static async Task<List<T>> ToListAsync<T>(this Task<IEnumerable<T>> x) => (await x).ToList();
}