using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TurkeyMapApp.Models;
using TurkeyMapApp.Services;

namespace TurkeyMapApp.Controllers;

public class HomeController : Controller
{
    private readonly ICityService _cityService;

    public HomeController(ICityService cityService)
    {
        _cityService = cityService;
    }

    public async Task<IActionResult> Index()
    {
        var cities = await _cityService.GetAllCitiesAsync();
        var placeCounts = (await _cityService.GetPlaceCountsAsync())
            .ToDictionary(c => c.Code.ToUpperInvariant(), c => c.PlaceCount);

        var viewModel = new MapViewModel
        {
            Cities = cities
        };

        ViewBag.CityDataJson = JsonSerializer.Serialize(cities.Select(c => new
        {
            c.Code,
            c.Name,
            c.PlateCode,
            c.Population,
            c.Region,
            PlaceCount = placeCounts.GetValueOrDefault(c.Code, 0)
        }));

        return View(viewModel);
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
