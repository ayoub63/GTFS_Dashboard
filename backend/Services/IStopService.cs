using GtfsDashboard.Api.DTOs;

namespace GtfsDashboard.Api.Services;

public interface IStopService
{
    Task<PagedResponse<StopDto>> SearchAsync(string? search, int limit, int offset, CancellationToken ct);
    Task<StopDto?> GetByIdAsync(string id, CancellationToken ct);
    Task<IReadOnlyCollection<StopDto>> GetNearbyAsync(double lat, double lon, int radiusM, int limit, CancellationToken ct);
    Task<IReadOnlyCollection<StopDto>> GetInBboxAsync(double minLat, double minLon, double maxLat, double maxLon, int limit, CancellationToken ct);
    Task<IReadOnlyCollection<StopDto>> GetChildrenAsync(string stationId, CancellationToken ct);
}