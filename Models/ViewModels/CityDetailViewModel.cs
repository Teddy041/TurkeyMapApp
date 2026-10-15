using TurkeyMapApp.Models.Entities;

namespace TurkeyMapApp.Models.ViewModels;

public class CityDetailViewModel
{
    public City City { get; set; } = null!;
    public string CityImageUrl { get; set; } = string.Empty;
    public string CityImageFallback { get; set; } = "/images/placeholders/default.svg";
    public int TotalPlaces { get; set; }
    public IEnumerable<string> Categories { get; set; } = [];
}

public class PlaceDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string FallbackImageUrl { get; set; } = string.Empty;
}

public class CityPlaceCountDto
{
    public string Code { get; set; } = string.Empty;
    public int PlateCode { get; set; }
    public string Name { get; set; } = string.Empty;
    public int PlaceCount { get; set; }
}
