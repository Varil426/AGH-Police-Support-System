namespace Simulation.Application.Helpers;

public static class HelperExtension
{
    public static TValue? TryGet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : class
    {
        dictionary.TryGetValue(key, out var result);
        return result;
    }
    
    public static TValue? TryGetNullable<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : struct
    {
        if (dictionary.TryGetValue(key, out var result))
            return result;
        return null;
    }
    
    public static TValue? TryGet<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key) where TValue : class
    {
        dictionary.TryGetValue(key, out var result);
        return result;
    }
    
    public static TValue? TryGetNullable<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key) where TValue : struct
    {
        if (dictionary.TryGetValue(key, out var result))
            return result;
        return null;
    }
    
    public static async Task<List<T>> ToListAsync<T>(this Task<IEnumerable<T>> x) => (await x).ToList();
}