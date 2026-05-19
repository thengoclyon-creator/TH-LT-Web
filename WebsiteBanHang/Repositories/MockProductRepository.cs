using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Repositories
{
    public class MockProductRepository : IProductRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string SessionKey = "ProductList";

        public MockProductRepository(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession? Session => _httpContextAccessor.HttpContext?.Session;

        private List<Product> GetProductList()
        {
            if (Session == null)
            {
                return GetDefaultProducts();
            }

            var sessionData = Session.GetString(SessionKey);
            if (string.IsNullOrEmpty(sessionData))
            {
                var defaultList = GetDefaultProducts();
                SaveProductList(defaultList);
                return defaultList;
            }

            try
            {
                return JsonSerializer.Deserialize<List<Product>>(sessionData) ?? GetDefaultProducts();
            }
            catch
            {
                return GetDefaultProducts();
            }
        }

        private void SaveProductList(List<Product> products)
        {
            Session?.SetString(SessionKey, JsonSerializer.Serialize(products));
        }

        private List<Product> GetDefaultProducts()
        {
            return new List<Product>
            {
                new Product { 
                    Id = 1, 
                    Name = "iPhone 14 Pro", 
                    Price = 25000000, 
                    Description = "Điện thoại cao cấp của Apple", 
                    CategoryId = 1, 
                    ImageUrl = "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?q=80&w=600&auto=format&fit=crop",
                    ImageUrls = new List<string> {
                        "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?q=80&w=600&auto=format&fit=crop",
                        "https://images.unsplash.com/photo-1510557880182-3d4d3cba35a5?q=80&w=600&auto=format&fit=crop",
                        "https://images.unsplash.com/photo-1523206489230-c012c64b2b48?q=80&w=600&auto=format&fit=crop"
                    },
                    SubImageUrl = "https://images.unsplash.com/photo-1510557880182-3d4d3cba35a5?q=80&w=600&auto=format&fit=crop",
                    Size = "128GB, 256GB, 512GB"
                },
                new Product { 
                    Id = 2, 
                    Name = "Dell XPS 13", 
                    Price = 35000000, 
                    Description = "Laptop ultrabook thiết kế đẹp", 
                    CategoryId = 2, 
                    ImageUrl = "https://images.unsplash.com/photo-1496181133206-80ce9b88a853?q=80&w=600&auto=format&fit=crop",
                    ImageUrls = new List<string> {
                        "https://images.unsplash.com/photo-1496181133206-80ce9b88a853?q=80&w=600&auto=format&fit=crop",
                        "https://images.unsplash.com/photo-1588872657578-7efd1f1555ed?q=80&w=600&auto=format&fit=crop"
                    },
                    SubImageUrl = "https://images.unsplash.com/photo-1588872657578-7efd1f1555ed?q=80&w=600&auto=format&fit=crop",
                    Size = "8GB RAM, 16GB RAM"
                },
                new Product { 
                    Id = 3, 
                    Name = "AirPods Pro 2", 
                    Price = 6000000, 
                    Description = "Tai nghe chống ồn chủ động", 
                    CategoryId = 3, 
                    ImageUrl = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?q=80&w=600&auto=format&fit=crop",
                    ImageUrls = new List<string> {
                        "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?q=80&w=600&auto=format&fit=crop",
                        "https://images.unsplash.com/photo-1588444839799-eaa432b87d20?q=80&w=600&auto=format&fit=crop"
                    },
                    SubImageUrl = "https://images.unsplash.com/photo-1588444839799-eaa432b87d20?q=80&w=600&auto=format&fit=crop",
                    Size = "Standard"
                },
                new Product { 
                    Id = 4, 
                    Name = "Bàn phím cơ Keychron K2", 
                    Price = 2100000, 
                    Description = "Bàn phím cơ không dây layout 84 phím, switch Gateron, LED RGB đẹp mắt", 
                    CategoryId = 3, 
                    ImageUrl = "https://images.unsplash.com/photo-1618384887929-16ec33fab9ef?q=80&w=600&auto=format&fit=crop",
                    ImageUrls = new List<string> {
                        "https://images.unsplash.com/photo-1618384887929-16ec33fab9ef?q=80&w=600&auto=format&fit=crop",
                        "https://images.unsplash.com/photo-1587829741301-dc798b83add3?q=80&w=600&auto=format&fit=crop"
                    },
                    SubImageUrl = "https://images.unsplash.com/photo-1587829741301-dc798b83add3?q=80&w=600&auto=format&fit=crop",
                    Size = "Red Switch, Blue Switch, Brown Switch"
                },
                new Product { 
                    Id = 5, 
                    Name = "Chuột Logitech MX Master 3S", 
                    Price = 2800000, 
                    Description = "Chuột công thái học cao cấp dành cho lập trình viên và nhà thiết kế", 
                    CategoryId = 3, 
                    ImageUrl = "https://images.unsplash.com/photo-1615663245857-ac93bb7c39e7?q=80&w=600&auto=format&fit=crop",
                    ImageUrls = new List<string> {
                        "https://images.unsplash.com/photo-1615663245857-ac93bb7c39e7?q=80&w=600&auto=format&fit=crop",
                        "https://images.unsplash.com/photo-1625842268584-8f3290455655?q=80&w=600&auto=format&fit=crop"
                    },
                    SubImageUrl = "https://images.unsplash.com/photo-1625842268584-8f3290455655?q=80&w=600&auto=format&fit=crop",
                    Size = "Graphite, Pale Gray"
                },
                new Product { 
                    Id = 6, 
                    Name = "Màn hình Dell UltraSharp U2723QE", 
                    Price = 13500000, 
                    Description = "Màn hình chuyên nghiệp 27 inch 4K IPS Black, hỗ trợ USB-C Hub", 
                    CategoryId = 2, 
                    ImageUrl = "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf?q=80&w=600&auto=format&fit=crop",
                    ImageUrls = new List<string> {
                        "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf?q=80&w=600&auto=format&fit=crop",
                        "https://images.unsplash.com/photo-1547082299-de196ea013d6?q=80&w=600&auto=format&fit=crop"
                    },
                    SubImageUrl = "https://images.unsplash.com/photo-1547082299-de196ea013d6?q=80&w=600&auto=format&fit=crop",
                    Size = "27 inch 4K"
                },
                new Product { 
                    Id = 7, 
                    Name = "Samsung Galaxy S23 Ultra", 
                    Price = 22000000, 
                    Description = "Flagship cao cấp nhất của Samsung với bút S Pen và camera 200MP", 
                    CategoryId = 1, 
                    ImageUrl = "https://images.unsplash.com/photo-1610945265064-0e34e5519bbf?q=80&w=600&auto=format&fit=crop",
                    ImageUrls = new List<string> {
                        "https://images.unsplash.com/photo-1610945265064-0e34e5519bbf?q=80&w=600&auto=format&fit=crop",
                        "https://images.unsplash.com/photo-1580910051074-3eb694886505?q=80&w=600&auto=format&fit=crop"
                    },
                    SubImageUrl = "https://images.unsplash.com/photo-1580910051074-3eb694886505?q=80&w=600&auto=format&fit=crop",
                    Size = "256GB, 512GB, 1TB"
                },
                new Product { 
                    Id = 8, 
                    Name = "Apple MacBook Pro 14 M2 Pro", 
                    Price = 48000000, 
                    Description = "Laptop hiệu năng cao cho người dùng chuyên nghiệp với vi xử lý M2 Pro", 
                    CategoryId = 2, 
                    ImageUrl = "https://images.unsplash.com/photo-1517336714731-489689fd1ca8?q=80&w=600&auto=format&fit=crop",
                    ImageUrls = new List<string> {
                        "https://images.unsplash.com/photo-1517336714731-489689fd1ca8?q=80&w=600&auto=format&fit=crop",
                        "https://images.unsplash.com/photo-1611186871348-b1ce696e52c9?q=80&w=600&auto=format&fit=crop"
                    },
                    SubImageUrl = "https://images.unsplash.com/photo-1611186871348-b1ce696e52c9?q=80&w=600&auto=format&fit=crop",
                    Size = "16GB RAM / 512GB SSD, 32GB RAM / 1TB SSD"
                }
            };
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return GetProductList();
        }

        public Product GetProductById(int id)
        {
            return GetProductList().FirstOrDefault(p => p.Id == id);
        }

        public void AddProduct(Product product)
        {
            var list = GetProductList();
            product.Id = list.Any() ? list.Max(p => p.Id) + 1 : 1;
            list.Add(product);
            SaveProductList(list);
        }

        public void UpdateProduct(Product product)
        {
            var list = GetProductList();
            var index = list.FindIndex(p => p.Id == product.Id);
            if (index != -1)
            {
                list[index] = product;
                SaveProductList(list);
            }
        }

        public void DeleteProduct(int id)
        {
            var list = GetProductList();
            var product = list.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                list.Remove(product);
                SaveProductList(list);
            }
        }
    }
}
