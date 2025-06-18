using ConsentFormEngine.Business.DTOs;
using ConsentFormEngine.Core.Utilities;
using ConsentFormEngine.Entities.Entities;

namespace ConsentFormEngine.Business.Abstract
{
    public interface ICityService
    {
        Task<DataResult<List<CityDto>>> GetCitiesByCountryIdAsync(int countryId);
    }
}
