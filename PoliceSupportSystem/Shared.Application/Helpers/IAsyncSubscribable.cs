namespace Shared.Application.Helpers;

public interface IAsyncSubscribable<T>
{
    void Subscribe(Func<T, Task> callback);
}