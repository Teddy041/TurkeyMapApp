namespace TurkeyMapApp.Models.Seed;

public class SeedDataModel
{
    public List<CitySeedItem> Cities { get; set; } = [];
    public List<PlaceSeedItem> Places { get; set; } = [];
}

public class CitySeedItem
{
    public int PlateCode { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public long Population { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}

public class PlaceSeedItem
{
    public int PlateCode { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}
