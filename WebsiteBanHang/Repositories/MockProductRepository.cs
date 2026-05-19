using System.Collections.Generic;
using System.Linq;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Repositories
{
    public class MockProductRepository : IProductRepository
    {
        private List<Product> _productList;

        public MockProductRepository()
        {
            _productList = new List<Product>
            {
                new Product { Id = 1, Name = "iPhone 14 Pro", Price = 25000000, Description = "Điện thoại cao cấp của Apple", CategoryId = 1 },
                new Product { Id = 2, Name = "Dell XPS 13", Price = 35000000, Description = "Laptop ultrabook thiết kế đẹp", CategoryId = 2 },
                new Product { Id = 3, Name = "AirPods Pro 2", Price = 6000000, Description = "Tai nghe chống ồn chủ động", CategoryId = 3 }
            };
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return _productList;
        }

        public Product GetProductById(int id)
        {
            return _productList.FirstOrDefault(p => p.Id == id);
        }

        public void AddProduct(Product product)
        {
            product.Id = _productList.Max(p => p.Id) + 1;
            _productList.Add(product);
        }

        public void UpdateProduct(Product product)
        {
            var index = _productList.FindIndex(p => p.Id == product.Id);
            if (index != -1)
            {
                _productList[index] = product;
            }
        }

        public void DeleteProduct(int id)
        {
            var product = _productList.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _productList.Remove(product);
            }
        }
    }
}
