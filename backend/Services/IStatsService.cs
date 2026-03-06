using GtfsDashboard.Api.DTOs;

namespace GtfsDashboard.Api.Services;

public interface IStatsService
{
    Task<IReadOnlyCollection<TopStopDto>> GetTopStopsAsync(DateOnly? date, DayOfWeek? weekday, int limit, CancellationToken ct);
    Task<IReadOnlyCollection<PeakHourDto>> GetPeakHoursAsync(DateOnly? date, DayOfWeek? weekday, CancellationToken ct);
    Task<IReadOnlyCollection<RouteByStopDto>> GetRoutesByStopAsync(string stopId, DateOnly? date, DayOfWeek? weekday, CancellationToken ct);
}