using System.Collections.Generic;
using System.Linq;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Repositories
{
    public class MockCategoryRepository : ICategoryRepository
    {
        private List<Category> _categoryList;

        public MockCategoryRepository()
        {
            _categoryList = new List<Category>
            {
                new Category { Id = 1, Name = "Điện thoại" },
                new Category { Id = 2, Name = "Laptop" },
                new Category { Id = 3, Name = "Phụ kiện" }
            };
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return _categoryList;
        }

        public Category GetCategoryById(int id)
        {
            return _categoryList.FirstOrDefault(c => c.Id == id);
        }
    }
}
