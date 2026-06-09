using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebsiteBanHang.Models;
using WebsiteBanHang.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "Product", new { area = "" });
        }

        public IActionResult Add()
        {
            var categories = _categoryRepository.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile? image, List<IFormFile>? images, IFormFile? subImage)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    product.ImageUrl = await SaveImage(image);
                }

                if (images != null && images.Count > 0)
                {
                    product.ImageUrls = new List<string>();
                    foreach (var img in images)
                    {
                        var url = await SaveImage(img);
                        product.ImageUrls.Add(url);
                    }
                }

                if (subImage != null)
                {
                    product.SubImageUrl = await SaveImage(subImage);
                }

                _productRepository.AddProduct(product);
                TempData["Message"] = "Thêm sản phẩm thành công!";
                return RedirectToAction("Index", "Product", new { area = "" });
            }
            
            var categories = _categoryRepository.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }

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

        [HttpPost]
        public async Task<IActionResult> Update(Product product, IFormFile? image, List<IFormFile>? images, IFormFile? subImage, string? existingImageUrlsJson)
        {
            if (ModelState.IsValid)
            {
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

                if (image != null)
                {
                    product.ImageUrl = await SaveImage(image);
                }

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

                if (subImage != null)
                {
                    product.SubImageUrl = await SaveImage(subImage);
                }

                _productRepository.UpdateProduct(product);
                TempData["Message"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction("Index", "Product", new { area = "" });
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

        public IActionResult Delete(int id)
        {
            var product = _productRepository.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _productRepository.DeleteProduct(id);
            TempData["Message"] = "Xóa sản phẩm thành công!";
            return RedirectToAction("Index", "Product", new { area = "" });
        }
    }
}
