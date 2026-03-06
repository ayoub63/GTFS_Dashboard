using Microsoft.EntityFrameworkCore;
using GtfsDashboard.Api.Models;
using Route = GtfsDashboard.Api.Models.Route;

namespace GtfsDashboard.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Stop> Stops => Set<Stop>();
    public DbSet<Route> Routes => Set<Route>();
    public DbSet<Trip> Trips => Set<Trip>();
    public DbSet<StopTime> StopTimes => Set<StopTime>();
    public DbSet<CalendarService> CalendarServices => Set<CalendarService>();
    public DbSet<CalendarDate> CalendarDates => Set<CalendarDate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Stop>().HasIndex(x => x.StopName);
        modelBuilder.Entity<Stop>().HasIndex(x => x.ParentStation);
        modelBuilder.Entity<Stop>().HasIndex(x => new { x.StopLat, x.StopLon });

        modelBuilder.Entity<StopTime>().HasIndex(x => x.StopId);
        modelBuilder.Entity<StopTime>().HasIndex(x => x.TripId);
        modelBuilder.Entity<Trip>().HasIndex(x => x.ServiceId);
        modelBuilder.Entity<Trip>().HasIndex(x => x.RouteId);
        modelBuilder.Entity<CalendarDate>().HasIndex(x => new { x.ServiceId, x.Date });
    }
}