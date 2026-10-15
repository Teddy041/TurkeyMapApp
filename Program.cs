using Microsoft.EntityFrameworkCore;
using TurkeyMapApp.Data;
using TurkeyMapApp.Repositories;
using TurkeyMapApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseLazyLoadingProxies()
           .UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IPlaceRepository, PlaceRepository>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<IDataSeeder, DataSeeder>();

var app = builder.Build();

// Veritabanı migration + seed (aynı anda tek instance çalıştırın)
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    logger.LogInformation("Veritabanı kontrol ediliyor...");
    await DatabaseInitializer.ApplyMigrationsAsync(db, app.Environment, logger);
    logger.LogInformation("Veritabanı hazır.");

    var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
    await seeder.SeedAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseRouting();
app.UseAuthorization();

app.UseStaticFiles();
app.MapStaticAssets();

app.MapControllers();

app.MapControllerRoute(
    name: "cityDetails",
    pattern: "City/Details/{plateCode:int}",
    defaults: new { controller = "City", action = "Details" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
