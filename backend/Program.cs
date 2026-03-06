using GtfsDashboard.Api.Data;
using GtfsDashboard.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default") ?? "Data Source=gtfs.db"));

builder.Services.AddScoped<IStopService, StopService>();
builder.Services.AddScoped<IStatsService, StatsService>();
builder.Services.AddScoped<IGtfsImportService, GtfsImportService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    var importer = scope.ServiceProvider.GetRequiredService<IGtfsImportService>();
    var dataFolderPath = app.Configuration["Gtfs:DataFolderPath"];
    
    if (!string.IsNullOrWhiteSpace(dataFolderPath) && Directory.Exists(dataFolderPath))
    {
       
        if (!db.Stops.Any()) 
        {
            Console.WriteLine("Database is empty. Starting GTFS import...");
            await importer.ImportFromFolderAsync(dataFolderPath, includeAnalyticsFiles: true, CancellationToken.None);
        }
        else
        {
            Console.WriteLine("GTFS data already exists in the database. Skipping import.");
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();