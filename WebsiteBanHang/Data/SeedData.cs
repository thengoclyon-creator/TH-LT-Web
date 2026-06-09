using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!roleManager.Roles.Any())
                {
                    roleManager.CreateAsync(new IdentityRole("Admin")).Wait();
                    roleManager.CreateAsync(new IdentityRole("User")).Wait();
                }

                if (!userManager.Users.Any(u => u.Email == "admin@mystore.com"))
                {
                    var user = new ApplicationUser 
                    { 
                        UserName = "admin@mystore.com", 
                        Email = "admin@mystore.com", 
                        FullName = "Quản trị viên", 
                        Address = "123 Đường Tôn Đức Thắng, TP. Hà Nội",
                        Age = 30
                    };
                    var result = userManager.CreateAsync(user, "Admin@123").Result;
                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Admin").Wait();
                    }
                }
                else
                {
                    // Đảm bảo tài khoản admin luôn có quyền Admin (trong trường hợp đã tạo tài khoản trước khi có code Role)
                    var existingAdmin = userManager.FindByEmailAsync("admin@mystore.com").Result;
                    if (existingAdmin != null && !userManager.IsInRoleAsync(existingAdmin, "Admin").Result)
                    {
                        userManager.AddToRoleAsync(existingAdmin, "Admin").Wait();
                    }
                }

                if (context.Products.Any() || context.Categories.Any())
                {
                    return;   // Dữ liệu đã được tạo
                }

                context.Categories.AddRange(
                    new Category { Name = "Điện thoại" },
                    new Category { Name = "Laptop" },
                    new Category { Name = "Phụ kiện" }
                );

                context.SaveChanges();

                var dienThoaiId = context.Categories.First(c => c.Name == "Điện thoại").Id;
                var laptopId = context.Categories.First(c => c.Name == "Laptop").Id;
                var phuKienId = context.Categories.First(c => c.Name == "Phụ kiện").Id;

                context.Products.AddRange(
                    new Product 
                    { 
                        Name = "iPhone 15 Pro Max 256GB", 
                        Price = 34990000, 
                        Description = "Sản phẩm mới nhất của Apple với khung Titan siêu nhẹ, camera tele 5x và vi xử lý A17 Pro mạnh mẽ.", 
                        CategoryId = dienThoaiId, 
                        ImageUrl = "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?q=80&w=600&auto=format&fit=crop"
                    },
                    new Product 
                    { 
                        Name = "Samsung Galaxy S24 Ultra 5G", 
                        Price = 33990000, 
                        Description = "Trang bị Galaxy AI thông minh, camera 200MP chụp đêm xuất sắc và bút S-Pen tích hợp.", 
                        CategoryId = dienThoaiId, 
                        ImageUrl = "https://images.unsplash.com/photo-1610945265064-0e34e5519bbf?q=80&w=600&auto=format&fit=crop"
                    },
                    new Product 
                    { 
                        Name = "MacBook Pro 14 inch M3 2023", 
                        Price = 39990000, 
                        Description = "Laptop cao cấp từ Apple với chip M3 siêu phân luồng, màn hình Liquid Retina XDR sắc nét 120Hz.", 
                        CategoryId = laptopId, 
                        ImageUrl = "https://images.unsplash.com/photo-1517336714731-489689fd1ca8?q=80&w=600&auto=format&fit=crop"
                    },
                    new Product 
                    { 
                        Name = "Tai nghe Bluetooth AirPods Pro Gen 2", 
                        Price = 6190000, 
                        Description = "Chống ồn chủ động (ANC) tốt gấp 2 lần thế hệ trước, tích hợp hộp sạc MagSafe có loa tìm kiếm.", 
                        CategoryId = phuKienId, 
                        ImageUrl = "https://images.unsplash.com/photo-1606220588913-b3aecb4b321a?q=80&w=600&auto=format&fit=crop"
                    },
                    new Product 
                    { 
                        Name = "Chuột không dây Logitech MX Master 3S", 
                        Price = 2490000, 
                        Description = "Chuột công thái học hàng đầu cho dân văn phòng và coder, cuộn MagSpeed siêu tốc, click êm ái.", 
                        CategoryId = phuKienId, 
                        ImageUrl = "https://images.unsplash.com/photo-1527864550417-7fd91fc51a46?q=80&w=600&auto=format&fit=crop"
                    }
                );
                
                context.SaveChanges();
            }
        }
    }
}
