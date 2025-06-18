using ConsentFormEngine.Business.Abstract;
using ConsentFormEngine.Core.Interfaces;
using ConsentFormEngine.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsentFormEngine.Core.Utilities;

namespace ConsentFormEngine.Business.Concrete
{
    public class CategoryManager: ICategoryService
    {
        private readonly IRepository<Category> _categoryRepository;

        public CategoryManager(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<DataResult<List<Category>>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return new DataResult<List<Category>>(categories, true, "Kategoriler listelendi.");
        }


    }
}
