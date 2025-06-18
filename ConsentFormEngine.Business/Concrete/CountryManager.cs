using ConsentFormEngine.Business.Abstract;
using ConsentFormEngine.Business.DTOs;
using ConsentFormEngine.Core.Interfaces;
using ConsentFormEngine.Core.Utilities;
using ConsentFormEngine.Entities.Entities;

namespace ConsentFormEngine.Business.Concrete
{
    public class CountryManager : ICountryService
    {
        private readonly IRepository<Country> _countryRepository;

        public CountryManager(IRepository<Country> countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public async Task<DataResult<CountryDto>> GetByIdAsync(Guid id)
        {
            var country = await _countryRepository.GetByIdAsync(id);

            if (country == null)
                return new DataResult<CountryDto>(null, false, "Ülke bulunamadı.");

            var countryDto = new CountryDto
            {
                Id = country.Id,
                Name = country.Title
            };

            return new DataResult<CountryDto>(countryDto, true, "Ülke başarıyla bulundu.");
        }
        public async Task<DataResult<List<CountryDto>>> GetAllAsync()
        {
            var countries = await _countryRepository.GetAllAsync();

            var countryDtos = countries.Select(c => new CountryDto
            {
                Id = c.Id,
                Name = c.Title
            }).ToList();

            return new DataResult<List<CountryDto>>(countryDtos, true, "Ülkeler başarıyla listelendi.");
        }

    }
}
