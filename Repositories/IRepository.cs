using TurkeyMapApp.Models.Entities;
using TurkeyMapApp.Models.ViewModels;

namespace TurkeyMapApp.Repositories;

public interface ICityRepository
{
    Task<IEnumerable<City>> GetAllAsync();
    Task<City?> GetByPlateCodeAsync(int plateCode);
    Task<City?> GetByCodeAsync(string code);
    Task<IEnumerable<CityPlaceCountDto>> GetPlaceCountsAsync();
}

public interface IPlaceRepository
{
    Task<IEnumerable<Place>> GetByCityIdAsync(int cityId);
    Task<IEnumerable<PlaceDto>> GetDtosByPlateCodeAsync(int plateCode);
    Task<IEnumerable<string>> GetCategoriesByCityIdAsync(int cityId);
    Task<int> GetCountByCityIdAsync(int cityId);
}
