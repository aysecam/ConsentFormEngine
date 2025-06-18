using ConsentFormEngine.Business.Abstract;
using ConsentFormEngine.Business.DTOs;
using ConsentFormEngine.Core.Interfaces;
using ConsentFormEngine.Core.Utilities;
using ConsentFormEngine.Entities.Entities;

namespace ConsentFormEngine.Business.Concrete
{
    public class CityManager : ICityService
    {
        private readonly IRepository<City> _cityRepository;


        public CityManager(IRepository<City> cityRepository)
        {
            _cityRepository = cityRepository;
        }

        public async Task<DataResult<List<CityDto>>> GetCitiesByCountryIdAsync(int countryId)
        {
            var cities = await _cityRepository.GetListAsync(c => c.CountryId == countryId);

            if (cities == null || !cities.Any())
                return new DataResult<List<CityDto>>(null, false, "Bu ülkeye ait şehir bulunamadı.");

            var cityDtos = cities.Select(city => new CityDto
            {
                Id = city.Id,
                Name = city.Title
            }).ToList();

            return new DataResult<List<CityDto>>(cityDtos, true, "Şehirler başarıyla listelendi.");
        }

    }
}
