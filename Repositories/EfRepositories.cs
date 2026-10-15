using Microsoft.EntityFrameworkCore;
using TurkeyMapApp.Data;
using TurkeyMapApp.Helpers;
using TurkeyMapApp.Models.Entities;
using TurkeyMapApp.Models.ViewModels;

namespace TurkeyMapApp.Repositories;

public class CityRepository : ICityRepository
{
    private readonly ApplicationDbContext _context;

    public CityRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<City>> GetAllAsync() =>
        await _context.Cities
            .AsNoTracking()
            .OrderBy(c => c.PlateCode)
            .ToListAsync();

    public async Task<City?> GetByPlateCodeAsync(int plateCode) =>
        await _context.Cities
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.PlateCode == plateCode);

    public async Task<City?> GetByCodeAsync(string code) =>
        await _context.Cities
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code == code.ToUpperInvariant());

    public async Task<IEnumerable<CityPlaceCountDto>> GetPlaceCountsAsync() =>
        await _context.Cities
            .AsNoTracking()
            .Select(c => new CityPlaceCountDto
            {
                Code = c.Code,
                PlateCode = c.PlateCode,
                Name = c.Name,
                PlaceCount = c.Places.Count
            })
            .ToListAsync();
}

public class PlaceRepository : IPlaceRepository
{
    private readonly ApplicationDbContext _context;

    public PlaceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Place>> GetByCityIdAsync(int cityId) =>
        await _context.Places
            .AsNoTracking()
            .Where(p => p.CityId == cityId)
            .OrderBy(p => p.Name)
            .ToListAsync();

    public async Task<IEnumerable<PlaceDto>> GetDtosByPlateCodeAsync(int plateCode) =>
        (await _context.Places
            .AsNoTracking()
            .Where(p => p.City.PlateCode == plateCode)
            .OrderBy(p => p.Name)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Description,
                p.Latitude,
                p.Longitude,
                p.Address,
                p.Category
            })
            .ToListAsync())
            .Select(p => new PlaceDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                Address = p.Address,
                Category = p.Category,
                ImageUrl = ImageUrlHelper.GetPlaceImageUrl(p.Id, p.Name, p.Category),
                FallbackImageUrl = ImageUrlHelper.GetCategoryFallback(p.Category)
            });

    public async Task<IEnumerable<string>> GetCategoriesByCityIdAsync(int cityId) =>
        await _context.Places
            .AsNoTracking()
            .Where(p => p.CityId == cityId)
            .Select(p => p.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();

    public async Task<int> GetCountByCityIdAsync(int cityId) =>
        await _context.Places.CountAsync(p => p.CityId == cityId);
}
