using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TurkeyMapApp.Data;
using TurkeyMapApp.Models.Entities;
using TurkeyMapApp.Models.Seed;

namespace TurkeyMapApp.Services;

public interface IDataSeeder
{
    Task SeedAsync();
}

/// <summary>
/// JSON seed dosyasından veritabanını doldurur.
/// </summary>
public class DataSeeder : IDataSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(
        ApplicationDbContext context,
        IWebHostEnvironment environment,
        ILogger<DataSeeder> logger)
    {
        _context = context;
        _environment = environment;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        if (await _context.Cities.AnyAsync())
        {
            _logger.LogInformation("Veritabanı zaten dolu, seed atlanıyor.");
            return;
        }

        var seedPath = Path.Combine(_environment.ContentRootPath, "Data", "seed-data.json");
        if (!File.Exists(seedPath))
        {
            _logger.LogError("Seed dosyası bulunamadı: {Path}", seedPath);
            throw new FileNotFoundException("Seed dosyası bulunamadı.", seedPath);
        }

        var json = await File.ReadAllTextAsync(seedPath);
        var seedData = JsonSerializer.Deserialize<SeedDataModel>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (seedData?.Cities == null || seedData.Places == null)
        {
            throw new InvalidOperationException("Seed dosyası geçersiz format.");
        }

        _logger.LogInformation("Seed başlıyor: {CityCount} il, {PlaceCount} yer",
            seedData.Cities.Count, seedData.Places.Count);

        var cityMap = new Dictionary<int, City>();

        foreach (var citySeed in seedData.Cities.OrderBy(c => c.PlateCode))
        {
            var city = new City
            {
                PlateCode = citySeed.PlateCode,
                Name = citySeed.Name,
                Code = citySeed.Code.ToUpperInvariant(),
                Region = citySeed.Region,
                Population = citySeed.Population,
                ImageUrl = citySeed.ImageUrl
            };
            _context.Cities.Add(city);
            cityMap[citySeed.PlateCode] = city;
        }

        await _context.SaveChangesAsync();

        // CityId'leri yeniden yükle
        var savedCities = await _context.Cities.ToDictionaryAsync(c => c.PlateCode);

        foreach (var placeSeed in seedData.Places)
        {
            if (!savedCities.TryGetValue(placeSeed.PlateCode, out var city))
                continue;

            _context.Places.Add(new Place
            {
                CityId = city.Id,
                Name = placeSeed.Name,
                Description = placeSeed.Description,
                Latitude = placeSeed.Latitude,
                Longitude = placeSeed.Longitude,
                Address = placeSeed.Address,
                Category = placeSeed.Category,
                ImageUrl = placeSeed.ImageUrl
            });
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Seed tamamlandı.");
    }
}
