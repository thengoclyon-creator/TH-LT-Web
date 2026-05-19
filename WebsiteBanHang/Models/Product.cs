using System.ComponentModel.DataAnnotations;

namespace WebsiteBanHang.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Tên sản phẩm phải từ 3 đến 100 ký tự")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Giá là bắt buộc")]
        [Range(0.01, 1000000000, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal? Price { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public List<string>? ImageUrls { get; set; } = new List<string>();

        public string? SubImageUrl { get; set; }

        public string? Size { get; set; }

        [Required(ErrorMessage = "Danh mục là bắt buộc")]
        public int? CategoryId { get; set; }
    }
}
