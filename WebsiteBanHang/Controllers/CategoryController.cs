using Microsoft.AspNetCore.Mvc;
using WebsiteBanHang.Models;
using WebsiteBanHang.Repositories;

namespace WebsiteBanHang.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // Index: Hiển thị danh sách tất cả danh mục
        public IActionResult Index()
        {
            var categories = _categoryRepository.GetAllCategories();
            return View(categories);
        }

        // Display: Xem chi tiết một danh mục
        public IActionResult Display(int id)
        {
            var category = _categoryRepository.GetCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // Add (GET): Hiển thị form thêm danh mục
        public IActionResult Add()
        {
            return View();
        }

        // Add (POST): Xử lý thêm danh mục
        [HttpPost]
        public IActionResult Add(Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryRepository.AddCategory(category);
                TempData["Message"] = "Thêm danh mục thành công!";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // Update (GET): Hiển thị form sửa danh mục
        public IActionResult Update(int id)
        {
            var category = _categoryRepository.GetCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // Update (POST): Xử lý cập nhật danh mục
        [HttpPost]
        public IActionResult Update(Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryRepository.UpdateCategory(category);
                TempData["Message"] = "Cập nhật danh mục thành công!";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // Delete (GET): Xác nhận xóa danh mục
        public IActionResult Delete(int id)
        {
            var category = _categoryRepository.GetCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // Delete (POST): Thực hiện xóa danh mục
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _categoryRepository.DeleteCategory(id);
            TempData["Message"] = "Xóa danh mục thành công!";
            return RedirectToAction("Index");
        }
    }
}
