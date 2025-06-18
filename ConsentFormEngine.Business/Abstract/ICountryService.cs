using ConsentFormEngine.Business.DTOs;
using ConsentFormEngine.Core.Utilities;
using ConsentFormEngine.Entities.Entities;

namespace ConsentFormEngine.Business.Abstract
{
    public interface ICountryService
    {
        Task<DataResult<CountryDto>> GetByIdAsync(Guid id);
        Task<DataResult<List<CountryDto>>> GetAllAsync();
    }
}
