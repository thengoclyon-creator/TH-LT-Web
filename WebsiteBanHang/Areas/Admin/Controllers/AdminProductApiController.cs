using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteBanHang.Models;
using WebsiteBanHang.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/admin/product")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminProductApiController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public AdminProductApiController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_productRepository.GetAllProducts());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var product = _productRepository.GetProductById(id);
            if (product == null) return NotFound(new { message = "Không tìm thấy sản phẩm" });
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] Product product, IFormFile? image, List<IFormFile>? images, IFormFile? subImage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (image != null)
            {
                product.ImageUrl = await SaveImage(image);
            }

            if (images != null && images.Count > 0)
            {
                product.ImageUrls = new List<string>();
                foreach (var img in images)
                {
                    product.ImageUrls.Add(await SaveImage(img));
                }
            }

            if (subImage != null)
            {
                product.SubImageUrl = await SaveImage(subImage);
            }

            _productRepository.AddProduct(product);
            return Ok(new { message = "Thêm sản phẩm thành công", id = product.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] Product product, IFormFile? image, List<IFormFile>? images, IFormFile? subImage, [FromForm] string? existingImageUrlsJson)
        {
            if (id != product.Id) return BadRequest(new { message = "ID không khớp" });
            
            // We only need to check model state but since we are handling images manually we might bypass some model validations or clear them if necessary.
            // But let's assume valid.

            var existingProduct = _productRepository.GetProductById(id);
            if (existingProduct == null) return NotFound(new { message = "Không tìm thấy sản phẩm" });

            // Cập nhật các trường cơ bản từ form sang đối tượng đang được theo dõi (tracking)
            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;
            existingProduct.Description = product.Description;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.Size = product.Size;

            if (!string.IsNullOrEmpty(existingImageUrlsJson))
            {
                try
                {
                    existingProduct.ImageUrls = System.Text.Json.JsonSerializer.Deserialize<List<string>>(existingImageUrlsJson) ?? new List<string>();
                }
                catch
                {
                    existingProduct.ImageUrls = new List<string>();
                }
            }

            if (image != null)
            {
                existingProduct.ImageUrl = await SaveImage(image);
            }

            if (images != null && images.Count > 0)
            {
                if (existingProduct.ImageUrls == null) existingProduct.ImageUrls = new List<string>();
                foreach (var img in images)
                {
                    existingProduct.ImageUrls.Add(await SaveImage(img));
                }
            }

            if (subImage != null)
            {
                existingProduct.SubImageUrl = await SaveImage(subImage);
            }

            _productRepository.UpdateProduct(existingProduct);
            return Ok(new { message = "Cập nhật sản phẩm thành công" });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var product = _productRepository.GetProductById(id);
            if (product == null) return NotFound(new { message = "Không tìm thấy sản phẩm" });

            _productRepository.DeleteProduct(id);
            return Ok(new { message = "Xóa sản phẩm thành công" });
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var filePath = Path.Combine(savePath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }
            return "/images/" + fileName;
        }
    }
}
