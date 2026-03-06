using System.Globalization;
using CsvHelper;
using GtfsDashboard.Api.Data;
using GtfsDashboard.Api.Models;
using Microsoft.EntityFrameworkCore;
using Route = GtfsDashboard.Api.Models.Route;


namespace GtfsDashboard.Api.Services;

public class GtfsImportService(AppDbContext db, ILogger<GtfsImportService> logger) : IGtfsImportService
{
    public async Task ImportFromFolderAsync(string dataFolderPath, bool includeAnalyticsFiles, CancellationToken ct)
    {
        if (!Directory.Exists(dataFolderPath))
        {
            throw new DirectoryNotFoundException($"GTFS data folder not found: {dataFolderPath}");
        }

        await using var tx = await db.Database.BeginTransactionAsync(ct);

        await ClearTablesAsync(includeAnalyticsFiles, ct);

        await ImportStopsAsync(dataFolderPath, ct);

        if (includeAnalyticsFiles)
        {
            await ImportRoutesAsync(dataFolderPath, ct);
            await ImportTripsAsync(dataFolderPath, ct);
            await ImportStopTimesAsync(dataFolderPath, ct);
            await ImportCalendarAsync(dataFolderPath, ct);
            await ImportCalendarDatesAsync(dataFolderPath, ct);
        }

        await tx.CommitAsync(ct);
        logger.LogInformation("GTFS import complete from folder {DataFolderPath}", dataFolderPath);
    }

    private async Task ClearTablesAsync(bool includeAnalyticsFiles, CancellationToken ct)
    {
        db.Stops.RemoveRange(db.Stops);
        if (includeAnalyticsFiles)
        {
            db.StopTimes.RemoveRange(db.StopTimes);
            db.Trips.RemoveRange(db.Trips);
            db.Routes.RemoveRange(db.Routes);
            db.CalendarDates.RemoveRange(db.CalendarDates);
            db.CalendarServices.RemoveRange(db.CalendarServices);
        }

        await db.SaveChangesAsync(ct);
    }

    private async Task ImportStopsAsync(string dataFolderPath, CancellationToken ct)
    {
        var rows = ReadRows(dataFolderPath, "stops.txt", required: true);
        var entities = rows.Select(r => new Stop
        {
            StopId = r.GetValueOrDefault("stop_id") ?? string.Empty,
            StopName = r.GetValueOrDefault("stop_name") ?? string.Empty,
            StopLat = ParseDouble(r.GetValueOrDefault("stop_lat")),
            StopLon = ParseDouble(r.GetValueOrDefault("stop_lon")),
            LocationType = ParseNullableInt(r.GetValueOrDefault("location_type")),
            ParentStation = NullIfEmpty(r.GetValueOrDefault("parent_station")),
            PlatformCode = NullIfEmpty(r.GetValueOrDefault("platform_code"))
        }).Where(s => !string.IsNullOrWhiteSpace(s.StopId)).ToList();

        db.Stops.AddRange(entities);
        await db.SaveChangesAsync(ct);
    }

    private async Task ImportRoutesAsync(string dataFolderPath, CancellationToken ct)
    {
        var rows = ReadRows(dataFolderPath, "routes.txt", required: false);
        var entities = rows.Select(r => new Route
        {
            RouteId = r.GetValueOrDefault("route_id") ?? string.Empty,
            AgencyId = NullIfEmpty(r.GetValueOrDefault("agency_id")),
            RouteShortName = NullIfEmpty(r.GetValueOrDefault("route_short_name")),
            RouteLongName = NullIfEmpty(r.GetValueOrDefault("route_long_name")),
            RouteType = ParseNullableInt(r.GetValueOrDefault("route_type"))
        }).Where(r => !string.IsNullOrWhiteSpace(r.RouteId)).ToList();

        db.Routes.AddRange(entities);
        await db.SaveChangesAsync(ct);
    }

    private async Task ImportTripsAsync(string dataFolderPath, CancellationToken ct)
    {
        var rows = ReadRows(dataFolderPath, "trips.txt", required: false);
        var entities = rows.Select(r => new Trip
        {
            TripId = r.GetValueOrDefault("trip_id") ?? string.Empty,
            RouteId = r.GetValueOrDefault("route_id") ?? string.Empty,
            ServiceId = r.GetValueOrDefault("service_id") ?? string.Empty,
            TripHeadsign = NullIfEmpty(r.GetValueOrDefault("trip_headsign")),
            DirectionId = ParseNullableInt(r.GetValueOrDefault("direction_id"))
        }).Where(t => !string.IsNullOrWhiteSpace(t.TripId)).ToList();

        db.Trips.AddRange(entities);
        await db.SaveChangesAsync(ct);
    }

    private async Task ImportStopTimesAsync(string dataFolderPath, CancellationToken ct)
    {
        var rows = ReadRows(dataFolderPath, "stop_times.txt", required: false);
        var entities = rows.Select(r => new StopTime
        {
            TripId = r.GetValueOrDefault("trip_id") ?? string.Empty,
            ArrivalTime = NullIfEmpty(r.GetValueOrDefault("arrival_time")),
            DepartureTime = NullIfEmpty(r.GetValueOrDefault("departure_time")),
            StopId = r.GetValueOrDefault("stop_id") ?? string.Empty,
            StopSequence = ParseInt(r.GetValueOrDefault("stop_sequence"))
        }).Where(st => !string.IsNullOrWhiteSpace(st.TripId) && !string.IsNullOrWhiteSpace(st.StopId)).ToList();

        db.StopTimes.AddRange(entities);
        await db.SaveChangesAsync(ct);
    }

    private async Task ImportCalendarAsync(string dataFolderPath, CancellationToken ct)
    {
        var rows = ReadRows(dataFolderPath, "calendar.txt", required: false);
        var entities = rows.Select(r => new CalendarService
        {
            ServiceId = r.GetValueOrDefault("service_id") ?? string.Empty,
            Monday = ParseInt(r.GetValueOrDefault("monday")),
            Tuesday = ParseInt(r.GetValueOrDefault("tuesday")),
            Wednesday = ParseInt(r.GetValueOrDefault("wednesday")),
            Thursday = ParseInt(r.GetValueOrDefault("thursday")),
            Friday = ParseInt(r.GetValueOrDefault("friday")),
            Saturday = ParseInt(r.GetValueOrDefault("saturday")),
            Sunday = ParseInt(r.GetValueOrDefault("sunday")),
            StartDate = r.GetValueOrDefault("start_date") ?? string.Empty,
            EndDate = r.GetValueOrDefault("end_date") ?? string.Empty
        }).Where(c => !string.IsNullOrWhiteSpace(c.ServiceId)).ToList();

        db.CalendarServices.AddRange(entities);
        await db.SaveChangesAsync(ct);
    }

    private async Task ImportCalendarDatesAsync(string dataFolderPath, CancellationToken ct)
    {
        var rows = ReadRows(dataFolderPath, "calendar_dates.txt", required: false);
        var entities = rows.Select(r => new CalendarDate
        {
            ServiceId = r.GetValueOrDefault("service_id") ?? string.Empty,
            Date = r.GetValueOrDefault("date") ?? string.Empty,
            ExceptionType = ParseInt(r.GetValueOrDefault("exception_type"))
        }).Where(c => !string.IsNullOrWhiteSpace(c.ServiceId)).ToList();

        db.CalendarDates.AddRange(entities);
        await db.SaveChangesAsync(ct);
    }

    private static List<Dictionary<string, string?>> ReadRows(string dataFolderPath, string fileName, bool required)
    {
        var filePath = Path.Combine(dataFolderPath, fileName);
        if (!File.Exists(filePath))
        {
            if (required)
            {
                throw new InvalidOperationException($"Missing required GTFS file: {filePath}");
            }

            return [];
        }

        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var rows = new List<Dictionary<string, string?>>();
        csv.Read();
        csv.ReadHeader();
        while (csv.Read())
        {
            var row = csv.HeaderRecord!.ToDictionary(h => h, h => csv.GetField(h));
            rows.Add(row);
        }

        return rows;
    }

    private static int ParseInt(string? value) => int.TryParse(value, out var x) ? x : 0;
    private static int? ParseNullableInt(string? value) => int.TryParse(value, out var x) ? x : null;
    private static double ParseDouble(string? value) => double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var x) ? x : 0d;
    private static string? NullIfEmpty(string? value) => string.IsNullOrWhiteSpace(value) ? null : value;
}