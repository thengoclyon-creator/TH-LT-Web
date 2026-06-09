using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebsiteBanHang.Extensions;
using WebsiteBanHang.Models;
using WebsiteBanHang.Repositories;

namespace WebsiteBanHang.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartApiController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private const string CART_KEY = "shopping_cart";

        public CartApiController(IProductRepository productRepository, UserManager<ApplicationUser> userManager)
        {
            _productRepository = productRepository;
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

        [HttpGet]
        public IActionResult GetCart()
        {
            var cart = HttpContext.Session.GetJson<List<CartItem>>(CART_KEY) ?? new List<CartItem>();
            return Ok(cart);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] CartRequestDto request)
        {
            var product = _productRepository.GetProductById(request.ProductId);
            if (product == null) return NotFound(new { message = "Sản phẩm không tồn tại" });

            var cart = HttpContext.Session.GetJson<List<CartItem>>(CART_KEY) ?? new List<CartItem>();
            var cartItem = cart.FirstOrDefault(c => c.ProductId == request.ProductId);

            if (cartItem != null)
            {
                cartItem.Quantity += request.Quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name ?? string.Empty,
                    Price = product.Price ?? 0,
                    Quantity = request.Quantity,
                    ImageUrl = product.ImageUrl
                });
            }

            HttpContext.Session.SetJson(CART_KEY, cart);
            await SaveCartToDbAsync(cart);

            return Ok(new { message = "Thêm vào giỏ hàng thành công", cartSize = cart.Sum(c => c.Quantity) });
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateQuantity([FromBody] CartRequestDto request)
        {
            var cart = HttpContext.Session.GetJson<List<CartItem>>(CART_KEY);
            if (cart != null)
            {
                var cartItem = cart.FirstOrDefault(c => c.ProductId == request.ProductId);
                if (cartItem != null)
                {
                    cartItem.Quantity = request.Quantity > 0 ? request.Quantity : 1;
                    HttpContext.Session.SetJson(CART_KEY, cart);
                    await SaveCartToDbAsync(cart);
                    return Ok(new { message = "Cập nhật thành công" });
                }
            }
            return BadRequest(new { message = "Không tìm thấy sản phẩm trong giỏ" });
        }

        [HttpDelete("remove/{productId}")]
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
                    return Ok(new { message = "Đã xóa sản phẩm khỏi giỏ" });
                }
            }
            return NotFound(new { message = "Không tìm thấy sản phẩm" });
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> Clear()
        {
            HttpContext.Session.Remove(CART_KEY);
            await SaveCartToDbAsync(new List<CartItem>());
            return Ok(new { message = "Đã xóa toàn bộ giỏ hàng" });
        }
    }

    public class CartRequestDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
