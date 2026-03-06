namespace GtfsDashboard.Api.DTOs;
public record StopDto(
    string StopId,
    string StopName,
    double StopLat,
    double StopLon,
    int? LocationType,
    string? ParentStation,
    string? PlatformCode);

public record PagedResponse<T>(IReadOnlyCollection<T> Items, int Total, int Limit, int Offset);

public record TopStopDto(string StopId, string StopName, int Count);
public record PeakHourDto(int Hour, int Count);
public record RouteByStopDto(string RouteId, string? RouteShortName, string? RouteLongName, int? RouteType, int Count);