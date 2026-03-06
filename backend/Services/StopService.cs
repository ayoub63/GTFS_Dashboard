using GtfsDashboard.Api.Data;
using GtfsDashboard.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GtfsDashboard.Api.Services;

public class StopService(AppDbContext db) : IStopService
{
    public async Task<PagedResponse<StopDto>> SearchAsync(string? search, int limit, int offset, CancellationToken ct)
    {
        var query = db.Stops.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalized = search.Trim().ToLower();
            query = query.Where(s => s.StopName.ToLower().Contains(normalized) || s.StopId.Contains(normalized));
        }

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(x => x.StopName)
            .Skip(offset)
            .Take(limit)
            .Select(x => new StopDto(x.StopId, x.StopName, x.StopLat, x.StopLon, x.LocationType, x.ParentStation, x.PlatformCode))
            .ToListAsync(ct);

        return new PagedResponse<StopDto>(items, total, limit, offset);
    }

    public Task<StopDto?> GetByIdAsync(string id, CancellationToken ct) =>
        db.Stops.AsNoTracking()
            .Where(x => x.StopId == id)
            .Select(x => new StopDto(x.StopId, x.StopName, x.StopLat, x.StopLon, x.LocationType, x.ParentStation, x.PlatformCode))
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyCollection<StopDto>> GetNearbyAsync(double lat, double lon, int radiusM, int limit, CancellationToken ct)
    {
        const double meterToDegree = 1.0 / 111_320d;
        var delta = radiusM * meterToDegree;

        var candidates = await db.Stops.AsNoTracking()
            .Where(s => s.StopLat >= lat - delta && s.StopLat <= lat + delta && s.StopLon >= lon - delta && s.StopLon <= lon + delta)
            .Select(x => new StopDto(x.StopId, x.StopName, x.StopLat, x.StopLon, x.LocationType, x.ParentStation, x.PlatformCode))
            .ToListAsync(ct);

        return candidates
            .Select(s => new { Stop = s, Distance = DistanceMeters(lat, lon, s.StopLat, s.StopLon) })
            .Where(x => x.Distance <= radiusM)
            .OrderBy(x => x.Distance)
            .Take(limit)
            .Select(x => x.Stop)
            .ToList();
    }

    public async Task<IReadOnlyCollection<StopDto>> GetInBboxAsync(double minLat, double minLon, double maxLat, double maxLon, int limit, CancellationToken ct)
    {
        return await db.Stops.AsNoTracking()
            .Where(s => s.StopLat >= minLat && s.StopLat <= maxLat && s.StopLon >= minLon && s.StopLon <= maxLon)
            .Take(limit)
            .Select(x => new StopDto(x.StopId, x.StopName, x.StopLat, x.StopLon, x.LocationType, x.ParentStation, x.PlatformCode))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyCollection<StopDto>> GetChildrenAsync(string stationId, CancellationToken ct)
    {
        return await db.Stops.AsNoTracking()
            .Where(s => s.ParentStation == stationId)
            .OrderBy(s => s.PlatformCode)
            .Select(x => new StopDto(x.StopId, x.StopName, x.StopLat, x.StopLon, x.LocationType, x.ParentStation, x.PlatformCode))
            .ToListAsync(ct);
    }

    private static double DistanceMeters(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371000;
        var dLat = DegreesToRadians(lat2 - lat1);
        var dLon = DegreesToRadians(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static double DegreesToRadians(double deg) => deg * Math.PI / 180;
}