using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebsiteBanHang.Models;
using WebsiteBanHang.Extensions;

namespace WebsiteBanHang.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string email, string password, string fullName, string address, int? age)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = email, Email = email, FullName = fullName, Address = address, Age = age };
                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    // Gán quyền User mặc định cho tài khoản mới
                    await _userManager.AddToRoleAsync(user, "User");
                    
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user != null)
                    {
                        // Lấy giỏ hàng từ Session hiện tại (khách ẩn danh)
                        var sessionCart = HttpContext.Session.GetJson<List<CartItem>>(CartController.CART_KEY) ?? new List<CartItem>();

                        // Lấy giỏ hàng từ Database của người dùng này
                        var dbCart = new List<CartItem>();
                        if (!string.IsNullOrEmpty(user.ShoppingCartJson))
                        {
                            try
                            {
                                dbCart = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(user.ShoppingCartJson) ?? new List<CartItem>();
                            }
                            catch { }
                        }

                        // Gộp giỏ hàng: nếu sản phẩm đã có thì cộng dồn số lượng
                        foreach (var item in sessionCart)
                        {
                            var existingItem = dbCart.FirstOrDefault(c => c.ProductId == item.ProductId);
                            if (existingItem != null)
                            {
                                existingItem.Quantity += item.Quantity;
                            }
                            else
                            {
                                dbCart.Add(item);
                            }
                        }

                        // Cập nhật lại Session
                        HttpContext.Session.SetJson(CartController.CART_KEY, dbCart);
                        
                        // Cập nhật lại vào DB
                        user.ShoppingCartJson = System.Text.Json.JsonSerializer.Serialize(dbCart);
                        await _userManager.UpdateAsync(user);
                    }

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không hợp lệ.");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear(); // Xóa giỏ hàng và các session khác khi đăng xuất
            return RedirectToAction("Index", "Home");
        }
    }
}
