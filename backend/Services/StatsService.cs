using GtfsDashboard.Api.Data;
using GtfsDashboard.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GtfsDashboard.Api.Services;

public class StatsService(AppDbContext db) : IStatsService
{
    public async Task<IReadOnlyCollection<TopStopDto>> GetTopStopsAsync(DateOnly? date, DayOfWeek? weekday, int limit, CancellationToken ct)
    {
        var filterByDate = date.HasValue || weekday.HasValue;
        var activeServiceIds = await GetActiveServiceIds(date, weekday, ct);
        var idsList = activeServiceIds.ToList();

        // Wenn nach Datum gefiltert wird, aber an dem Tag keine Services aktiv sind -> leeres Ergebnis
        if (filterByDate && idsList.Count == 0) return [];

        var query = from st in db.StopTimes.AsNoTracking()
                    join t in db.Trips.AsNoTracking() on st.TripId equals t.TripId
                    join s in db.Stops.AsNoTracking() on st.StopId equals s.StopId
                    select new { st, t, s };

        if (filterByDate)
        {
            query = query.Where(x => idsList.Contains(x.t.ServiceId));
        }

        var grouped = from x in query
                      group x.s by new { x.s.StopId, x.s.StopName } into g
                      orderby g.Count() descending
                      select new TopStopDto(g.Key.StopId, g.Key.StopName, g.Count());

        return await grouped.Take(limit).ToListAsync(ct);
    }

    public async Task<IReadOnlyCollection<PeakHourDto>> GetPeakHoursAsync(DateOnly? date, DayOfWeek? weekday, CancellationToken ct)
    {
        var filterByDate = date.HasValue || weekday.HasValue;
        var activeServiceIds = await GetActiveServiceIds(date, weekday, ct);
        var idsList = activeServiceIds.ToList();

        if (filterByDate && idsList.Count == 0) return [];

        var tripService = db.Trips.AsNoTracking();
        if (filterByDate)
        {
            tripService = tripService.Where(t => idsList.Contains(t.ServiceId));
        }

        var raw = await (from st in db.StopTimes.AsNoTracking()
                         join t in tripService on st.TripId equals t.TripId
                         where st.DepartureTime != null && st.DepartureTime != ""
                         select st.DepartureTime!).ToListAsync(ct);

        return raw
            .Where(t => t.Length >= 2)
            .Select(t => int.TryParse(t[..2], out var hour) ? hour : -1)
            .Where(h => h >= 0)
            .GroupBy(h => h)
            .OrderBy(g => g.Key)
            .Select(g => new PeakHourDto(g.Key, g.Count()))
            .ToList();
    }

    public async Task<IReadOnlyCollection<RouteByStopDto>> GetRoutesByStopAsync(string stopId, DateOnly? date, DayOfWeek? weekday, CancellationToken ct)
    {
        var filterByDate = date.HasValue || weekday.HasValue;
        var activeServiceIds = await GetActiveServiceIds(date, weekday, ct);
        var idsList = activeServiceIds.ToList();

        if (filterByDate && idsList.Count == 0) return [];

        var query = from st in db.StopTimes.AsNoTracking()
                    join t in db.Trips.AsNoTracking() on st.TripId equals t.TripId
                    join r in db.Routes.AsNoTracking() on t.RouteId equals r.RouteId
                    where st.StopId == stopId
                    select new { st, t, r };

        if (filterByDate)
        {
            query = query.Where(x => idsList.Contains(x.t.ServiceId));
        }

        var grouped = from x in query
                      group x.r by new { x.r.RouteId, x.r.RouteShortName, x.r.RouteLongName, x.r.RouteType } into g
                      orderby g.Count() descending
                      select new RouteByStopDto(g.Key.RouteId, g.Key.RouteShortName, g.Key.RouteLongName, g.Key.RouteType, g.Count());

        return await grouped.ToListAsync(ct);
    }

    private async Task<HashSet<string>> GetActiveServiceIds(DateOnly? date, DayOfWeek? weekday, CancellationToken ct)
    {
        if (date is null && weekday is null) return [];

        var targetDate = date ?? DateOnly.FromDateTime(DateTime.Today);
        var dow = weekday ?? targetDate.DayOfWeek;
        var dateString = targetDate.ToString("yyyyMMdd");

        // FIX: CompareTo wird von EF Core verstanden und in ein simples SQL '<=' und '>=' übersetzt
        var baseQuery = db.CalendarServices.AsNoTracking()
            .Where(c => c.StartDate.CompareTo(dateString) <= 0 &&
                        c.EndDate.CompareTo(dateString) >= 0)
            .Where(c => dow == DayOfWeek.Monday ? c.Monday == 1 :
                        dow == DayOfWeek.Tuesday ? c.Tuesday == 1 :
                        dow == DayOfWeek.Wednesday ? c.Wednesday == 1 :
                        dow == DayOfWeek.Thursday ? c.Thursday == 1 :
                        dow == DayOfWeek.Friday ? c.Friday == 1 :
                        dow == DayOfWeek.Saturday ? c.Saturday == 1 : c.Sunday == 1)
            .Select(c => c.ServiceId);

        var ids = (await baseQuery.ToListAsync(ct)).ToHashSet();

        var exceptions = await db.CalendarDates.AsNoTracking()
            .Where(cd => cd.Date == dateString)
            .ToListAsync(ct);

        foreach (var ex in exceptions)
        {
            if (ex.ExceptionType == 1) ids.Add(ex.ServiceId);
            if (ex.ExceptionType == 2) ids.Remove(ex.ServiceId);
        }

        return ids;
    }
}