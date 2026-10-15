using Microsoft.AspNetCore.Mvc;
using TurkeyMapApp.Services;

namespace TurkeyMapApp.Controllers.Api;

[ApiController]
[Route("api/cities")]
public class CitiesApiController : ControllerBase
{
    private readonly ICityService _cityService;

    public CitiesApiController(ICityService cityService)
    {
        _cityService = cityService;
    }

    /// <summary>
    /// Harita hover için tüm illerin gezilecek yer sayılarını döndürür (AJAX).
    /// </summary>
    [HttpGet("place-counts")]
    public async Task<IActionResult> GetPlaceCounts()
    {
        var counts = await _cityService.GetPlaceCountsAsync();
        return Ok(counts);
    }

    /// <summary>
    /// Belirli bir ilin gezilecek yerlerini lazy-load olarak döndürür (AJAX).
    /// </summary>
    [HttpGet("{plateCode:int}/places")]
    public async Task<IActionResult> GetPlaces(int plateCode)
    {
        var city = await _cityService.GetCityDetailAsync(plateCode);
        if (city == null)
            return NotFound();

        var places = await _cityService.GetPlacesByPlateCodeAsync(plateCode);
        return Ok(new
        {
            city = new
            {
                city.City.Name,
                city.City.PlateCode,
                city.City.ImageUrl,
                city.TotalPlaces
            },
            places
        });
    }
}
