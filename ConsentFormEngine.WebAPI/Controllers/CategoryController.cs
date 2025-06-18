using Microsoft.AspNetCore.Mvc;
using ConsentFormEngine.Business.Abstract;

namespace ConsentFormEngine.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController: ControllerBase
    {

        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var countries = await _categoryService.GetAllAsync();
            return Ok(countries);
        }
    }
}
