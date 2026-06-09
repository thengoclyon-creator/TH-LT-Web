using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebsiteBanHang.Data;
using WebsiteBanHang.Extensions;
using WebsiteBanHang.Models;
using WebsiteBanHang.Repositories;

namespace WebsiteBanHang.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public const string CART_KEY = "shopping_cart";

        public CartController(IProductRepository productRepository, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _productRepository = productRepository;
            _context = context;
            _userManager = userManager;
        }

        private async Task SaveCartToDbAsync(List<CartItem> cart)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    user.ShoppingCartJson = System.Text.Json.JsonSerializer.Serialize(cart);
                    await _userManager.UpdateAsync(user);
                }
            }
        }

        // Lấy danh sách sản phẩm trong giỏ hàng
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetJson<List<CartItem>>(CART_KEY) ?? new List<CartItem>();
            return View(cart);
        }

        // Thêm sản phẩm vào giỏ hàng
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var product = _productRepository.GetProductById(productId);
            if (product == null)
            {
                return NotFound("Sản phẩm không tồn tại");
            }

            var cart = HttpContext.Session.GetJson<List<CartItem>>(CART_KEY) ?? new List<CartItem>();
            var cartItem = cart.FirstOrDefault(c => c.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name ?? string.Empty,
                    Price = product.Price ?? 0,
                    Quantity = quantity,
                    ImageUrl = product.ImageUrl
                });
            }

            HttpContext.Session.SetJson(CART_KEY, cart);
            await SaveCartToDbAsync(cart);
            return RedirectToAction("Index");
        }

        // Cập nhật số lượng sản phẩm
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            var cart = HttpContext.Session.GetJson<List<CartItem>>(CART_KEY);
            if (cart != null)
            {
                var cartItem = cart.FirstOrDefault(c => c.ProductId == productId);
                if (cartItem != null)
                {
                    cartItem.Quantity = quantity > 0 ? quantity : 1;
                    HttpContext.Session.SetJson(CART_KEY, cart);
                    await SaveCartToDbAsync(cart);
                }
            }
            return RedirectToAction("Index");
        }

        // Xóa sản phẩm khỏi giỏ hàng
        public async Task<IActionResult> Remove(int productId)
        {
            var cart = HttpContext.Session.GetJson<List<CartItem>>(CART_KEY);
            if (cart != null)
            {
                var cartItem = cart.FirstOrDefault(c => c.ProductId == productId);
                if (cartItem != null)
                {
                    cart.Remove(cartItem);
                    HttpContext.Session.SetJson(CART_KEY, cart);
                    await SaveCartToDbAsync(cart);
                }
            }
            return RedirectToAction("Index");
        }

        // Xóa toàn bộ giỏ hàng
        public async Task<IActionResult> Clear()
        {
            HttpContext.Session.Remove(CART_KEY);
            await SaveCartToDbAsync(new List<CartItem>());
            return RedirectToAction("Index");
        }

        // --- Thanh toán ---

        [Authorize]
        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.GetJson<List<CartItem>>(CART_KEY);
            if (cart == null || cart.Count == 0)
            {
                return RedirectToAction("Index");
            }
            return View(cart);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ProcessCheckout(string shippingAddress, string district, string notes, string paymentMethod = "COD", decimal shippingFee = 0)
        {
            var cart = HttpContext.Session.GetJson<List<CartItem>>(CART_KEY);
            if (cart == null || cart.Count == 0) return RedirectToAction("Index");

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // Tính toán lại phí ship ở server để bảo mật
            decimal finalShippingFee = 0;
            switch (district)
            {
                case "Quận 1, TP.HCM":
                case "Quận 3, TP.HCM":
                    finalShippingFee = 15000;
                    break;
                case "Quận 10, TP.HCM":
                case "Quận Bình Thạnh, TP.HCM":
                    finalShippingFee = 20000;
                    break;
                case "TP. Thủ Đức, TP.HCM":
                    finalShippingFee = 30000;
                    break;
                case "Quận 9, TP.HCM":
                    finalShippingFee = 35000;
                    break;
                case "Huyện Bình Chánh, TP.HCM":
                    finalShippingFee = 50000;
                    break;
                case "Tỉnh/Thành Khác":
                    finalShippingFee = 70000;
                    break;
            }

            var fullAddress = string.IsNullOrWhiteSpace(district) ? shippingAddress : $"{shippingAddress}, {district}";

            var order = new Order
            {
                UserId = user.Id,
                ShippingAddress = fullAddress,
                Notes = notes,
                ShippingFee = finalShippingFee,
                PaymentMethod = paymentMethod,
                TotalPrice = cart.Sum(c => c.Total) + finalShippingFee
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in cart)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                };
                _context.OrderDetails.Add(orderDetail);
            }
            await _context.SaveChangesAsync();

            // Xóa giỏ hàng sau khi đặt thành công
            HttpContext.Session.Remove(CART_KEY);
            user.ShoppingCartJson = null;
            await _userManager.UpdateAsync(user);

            return RedirectToAction("CheckoutSuccess");
        }

        public IActionResult CheckoutSuccess()
        {
            return View();
        }
    }
}
