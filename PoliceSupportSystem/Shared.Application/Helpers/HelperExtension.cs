namespace Shared.Application.Helpers;

public static class HelperExtension
{
    public static TV? TryGet<TK, TV>(this IDictionary<TK, TV> dictionary, TK key)
    {
        dictionary.TryGetValue(key, out var result);
        return result;
    }
}