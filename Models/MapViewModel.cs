using TurkeyMapApp.Models.Entities;

namespace TurkeyMapApp.Models;

public class MapViewModel
{
    public IEnumerable<City> Cities { get; set; } = Enumerable.Empty<City>();

    public int TotalProvinces => Cities.Count();

    public long TotalPopulation => Cities.Sum(c => c.Population);
}
