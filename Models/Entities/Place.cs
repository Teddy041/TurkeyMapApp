namespace TurkeyMapApp.Models.Entities;

/// <summary>
/// Gezilecek yer entity'si.
/// </summary>
public class Place
{
    public int Id { get; set; }
    public int CityId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;

    public virtual City City { get; set; } = null!;
}
