using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteBanHang.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public decimal TotalPrice { get; set; }
        
        public decimal ShippingFee { get; set; } = 0;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
        public string ShippingAddress { get; set; } = string.Empty;

        public string? Notes { get; set; }

        public string Status { get; set; } = "Chờ xử lý"; // Chờ xử lý, Đã xác nhận, Đang giao, Hoàn thành, Đã hủy
        
        public string PaymentMethod { get; set; } = "COD"; // COD, Chuyển khoản

        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
