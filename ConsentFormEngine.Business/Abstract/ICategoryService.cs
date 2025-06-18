using ConsentFormEngine.Core.Utilities;
using ConsentFormEngine.Entities.Entities;

namespace ConsentFormEngine.Business.Abstract
{
    public interface ICategoryService
    {
        Task<DataResult<List<Category>>> GetAllAsync();

    }
}
