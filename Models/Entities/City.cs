namespace TurkeyMapApp.Models.Entities;

/// <summary>
/// Veritabanı il entity'si.
/// </summary>
public class City
{
    public int Id { get; set; }
    public int PlateCode { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public long Population { get; set; }
    public string ImageUrl { get; set; } = string.Empty;

    public virtual ICollection<Place> Places { get; set; } = new List<Place>();
}
