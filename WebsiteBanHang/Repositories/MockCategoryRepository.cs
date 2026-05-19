using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Repositories
{
    public class MockCategoryRepository : ICategoryRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string SessionKey = "CategoryList";

        public MockCategoryRepository(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession? Session => _httpContextAccessor.HttpContext?.Session;

        private List<Category> GetCategoryList()
        {
            if (Session == null)
            {
                return GetDefaultCategories();
            }

            var sessionData = Session.GetString(SessionKey);
            if (string.IsNullOrEmpty(sessionData))
            {
                var defaultList = GetDefaultCategories();
                SaveCategoryList(defaultList);
                return defaultList;
            }

            try
            {
                return JsonSerializer.Deserialize<List<Category>>(sessionData) ?? GetDefaultCategories();
            }
            catch
            {
                return GetDefaultCategories();
            }
        }

        private void SaveCategoryList(List<Category> categories)
        {
            Session?.SetString(SessionKey, JsonSerializer.Serialize(categories));
        }

        private List<Category> GetDefaultCategories()
        {
            return new List<Category>
            {
                new Category { Id = 1, Name = "Điện thoại" },
                new Category { Id = 2, Name = "Laptop" },
                new Category { Id = 3, Name = "Phụ kiện" }
            };
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return GetCategoryList();
        }

        public Category GetCategoryById(int id)
        {
            return GetCategoryList().FirstOrDefault(c => c.Id == id);
        }

        public void AddCategory(Category category)
        {
            var list = GetCategoryList();
            category.Id = list.Any() ? list.Max(c => c.Id) + 1 : 1;
            list.Add(category);
            SaveCategoryList(list);
        }

        public void UpdateCategory(Category category)
        {
            var list = GetCategoryList();
            var index = list.FindIndex(c => c.Id == category.Id);
            if (index != -1)
            {
                list[index] = category;
                SaveCategoryList(list);
            }
        }

        public void DeleteCategory(int id)
        {
            var list = GetCategoryList();
            var category = list.FirstOrDefault(c => c.Id == id);
            if (category != null)
            {
                list.Remove(category);
                SaveCategoryList(list);
            }
        }
    }
}
