using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebsiteBanHang.Models;
using WebsiteBanHang.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebsiteBanHang.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        // Index: Hiển thị danh sách tất cả sản phẩm
        public IActionResult Index()
        {
            var products = _productRepository.GetAllProducts();
            return View(products);
        }

        // Display: Hiển thị thông tin chi tiết của một sản phẩm
        public IActionResult Display(int id)
        {
            var product = _productRepository.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // Add (GET): Hiển thị form thêm sản phẩm
        public IActionResult Add()
        {
            var categories = _categoryRepository.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        // Add (POST): Xử lý thêm sản phẩm
        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile? image, List<IFormFile>? images, IFormFile? subImage)
        {
            if (ModelState.IsValid)
            {
                // Lưu ảnh đại diện chính
                if (image != null)
                {
                    product.ImageUrl = await SaveImage(image);
                }

                // Lưu danh sách nhiều ảnh chính
                if (images != null && images.Count > 0)
                {
                    product.ImageUrls = new List<string>();
                    foreach (var img in images)
                    {
                        var url = await SaveImage(img);
                        product.ImageUrls.Add(url);
                    }
                }

                // Lưu ảnh phụ
                if (subImage != null)
                {
                    product.SubImageUrl = await SaveImage(subImage);
                }

                _productRepository.AddProduct(product);
                TempData["Message"] = "Thêm sản phẩm thành công!";
                return RedirectToAction("Index");
            }
            
            var categories = _categoryRepository.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }

        // Update (GET): Hiển thị form cập nhật
        public IActionResult Update(int id)
        {
            var product = _productRepository.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }

            var categories = _categoryRepository.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // Update (POST): Xử lý cập nhật
        [HttpPost]
        public async Task<IActionResult> Update(Product product, IFormFile? image, List<IFormFile>? images, IFormFile? subImage, string? existingImageUrlsJson)
        {
            if (ModelState.IsValid)
            {
                // Giữ lại các ảnh chính cũ nếu không upload ảnh mới
                if (!string.IsNullOrEmpty(existingImageUrlsJson))
                {
                    try
                    {
                        product.ImageUrls = System.Text.Json.JsonSerializer.Deserialize<List<string>>(existingImageUrlsJson) ?? new List<string>();
                    }
                    catch
                    {
                        product.ImageUrls = new List<string>();
                    }
                }

                // Cập nhật ảnh đại diện chính
                if (image != null)
                {
                    product.ImageUrl = await SaveImage(image);
                }

                // Cập nhật thêm danh sách nhiều ảnh chính mới (nếu có)
                if (images != null && images.Count > 0)
                {
                    if (product.ImageUrls == null)
                    {
                        product.ImageUrls = new List<string>();
                    }
                    foreach (var img in images)
                    {
                        var url = await SaveImage(img);
                        product.ImageUrls.Add(url);
                    }
                }

                // Cập nhật ảnh phụ
                if (subImage != null)
                {
                    product.SubImageUrl = await SaveImage(subImage);
                }

                _productRepository.UpdateProduct(product);
                TempData["Message"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction("Index");
            }

            var categories = _categoryRepository.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var filePath = Path.Combine(savePath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }
            return "/images/" + fileName;
        }

        // Delete (GET): Xác nhận xóa
        public IActionResult Delete(int id)
        {
            var product = _productRepository.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // DeleteConfirmed: Xử lý xóa
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _productRepository.DeleteProduct(id);
            TempData["Message"] = "Xóa sản phẩm thành công!";
            return RedirectToAction("Index");
        }
    }
}
