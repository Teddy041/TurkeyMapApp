using Microsoft.AspNetCore.Mvc;
using TurkeyMapApp.Services;

namespace TurkeyMapApp.Controllers;

public class CityController : Controller
{
    private readonly ICityService _cityService;

    public CityController(ICityService cityService)
    {
        _cityService = cityService;
    }

    /// <summary>
    /// Şehir detay sayfası - /City/Details/{plakaKodu}
    /// </summary>
    [Route("City/Details/{plateCode:int}")]
    public async Task<IActionResult> Details(int plateCode)
    {
        var model = await _cityService.GetCityDetailAsync(plateCode);
        if (model == null)
            return NotFound();

        return View(model);
    }
}
