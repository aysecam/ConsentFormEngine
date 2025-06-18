using Microsoft.AspNetCore.Mvc;
using ConsentFormEngine.Business.Abstract;

namespace ConsentFormEngine.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CityController : ControllerBase
    {
        private readonly ICityService _cityService;

        public CityController(ICityService cityService)
        {
            _cityService = cityService;
        }

        [HttpGet("get-by-country/{countryId}")]
        public async Task<IActionResult> GetByCountry(int countryId)
        {
            var cities = await _cityService.GetCitiesByCountryIdAsync(countryId);
            return Ok(cities);
        }
    }

}
