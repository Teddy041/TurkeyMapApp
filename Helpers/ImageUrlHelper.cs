using System.Text.RegularExpressions;

namespace TurkeyMapApp.Helpers;

/// <summary>
/// Harici görsel URL'leri yerine güvenilir kaynak ve yerel yedek sağlar.
/// </summary>
public static class ImageUrlHelper
{
    private static readonly Dictionary<string, string> CategorySlugs = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Tarihi Yer"] = "tarihi-yer",
        ["Müze"] = "muze",
        ["Doğal Güzellik"] = "dogal-guzellik",
        ["Milli Park"] = "milli-park",
        ["Plaj"] = "plaj",
        ["Camii"] = "camii",
        ["Antik Kent"] = "antik-kent"
    };

    /// <summary>
    /// Gezilecek yer için deterministik, her zaman yüklenen fotoğraf URL'si.
    /// </summary>
    public static string GetPlaceImageUrl(int id, string name, string category)
    {
        var seed = CreateSeed($"place-{id}-{Normalize(name)}");
        return $"https://picsum.photos/seed/{seed}/800/450";
    }

    /// <summary>
    /// Şehir kapak fotoğrafı URL'si.
    /// </summary>
    public static string GetCityImageUrl(int plateCode, string name)
    {
        var seed = CreateSeed($"city-{plateCode}-{Normalize(name)}");
        return $"https://picsum.photos/seed/{seed}/1200/640";
    }

    /// <summary>
    /// Kategori bazlı yerel SVG yedek görseli.
    /// </summary>
    public static string GetCategoryFallback(string category)
    {
        var slug = CategorySlugs.GetValueOrDefault(category, "default");
        return $"/images/placeholders/{slug}.svg";
    }

    private static string CreateSeed(string input)
    {
        var normalized = Regex.Replace(input.ToLowerInvariant(), @"[^a-z0-9]+", "-").Trim('-');
        return Uri.EscapeDataString(normalized.Length > 0 ? normalized : "turkiye");
    }

    private static string Normalize(string value) =>
        value.Trim().ToLowerInvariant();
}
