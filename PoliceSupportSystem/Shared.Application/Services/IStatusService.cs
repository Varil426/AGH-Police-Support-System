namespace Shared.Application.Services;

public interface IStatusService
{
    Task AnnounceOnline();

    Task AnnounceOffline();
}