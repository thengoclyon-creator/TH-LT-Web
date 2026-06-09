using Microsoft.AspNetCore.Identity;

namespace WebsiteBanHang.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public int? Age { get; set; }
        public string? ShoppingCartJson { get; set; }
    }
}
