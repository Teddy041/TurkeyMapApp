using TurkeyMapApp.Helpers;
using TurkeyMapApp.Models.Entities;
using TurkeyMapApp.Models.ViewModels;
using TurkeyMapApp.Repositories;

namespace TurkeyMapApp.Services;

public interface ICityService
{
    Task<IEnumerable<City>> GetAllCitiesAsync();
    Task<CityDetailViewModel?> GetCityDetailAsync(int plateCode);
    Task<IEnumerable<CityPlaceCountDto>> GetPlaceCountsAsync();
    Task<IEnumerable<PlaceDto>> GetPlacesByPlateCodeAsync(int plateCode);
}

public class CityService : ICityService
{
    private readonly ICityRepository _cityRepository;
    private readonly IPlaceRepository _placeRepository;

    public CityService(ICityRepository cityRepository, IPlaceRepository placeRepository)
    {
        _cityRepository = cityRepository;
        _placeRepository = placeRepository;
    }

    public Task<IEnumerable<City>> GetAllCitiesAsync() =>
        _cityRepository.GetAllAsync();

    public async Task<CityDetailViewModel?> GetCityDetailAsync(int plateCode)
    {
        var city = await _cityRepository.GetByPlateCodeAsync(plateCode);
        if (city == null) return null;

        var totalPlaces = await _placeRepository.GetCountByCityIdAsync(city.Id);
        var categories = await _placeRepository.GetCategoriesByCityIdAsync(city.Id);

        return new CityDetailViewModel
        {
            City = city,
            CityImageUrl = ImageUrlHelper.GetCityImageUrl(city.PlateCode, city.Name),
            CityImageFallback = "/images/placeholders/default.svg",
            TotalPlaces = totalPlaces,
            Categories = categories
        };
    }

    public Task<IEnumerable<CityPlaceCountDto>> GetPlaceCountsAsync() =>
        _cityRepository.GetPlaceCountsAsync();

    public Task<IEnumerable<PlaceDto>> GetPlacesByPlateCodeAsync(int plateCode) =>
        _placeRepository.GetDtosByPlateCodeAsync(plateCode);
}
