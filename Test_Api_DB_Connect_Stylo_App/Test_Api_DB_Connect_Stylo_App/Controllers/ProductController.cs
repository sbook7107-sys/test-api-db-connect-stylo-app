using Microsoft.AspNetCore.Mvc;
using Test_Api_DB_Connect_Stylo_App.Services;

namespace Test_Api_DB_Connect_Stylo_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        // Endpoint cho màn hình Home (Tải 100 Biến thể)
        [HttpGet("home")] // Giả định bạn đã có một Endpoint gọi GetHomeScreenDataAsync
        public async Task<IActionResult> GetHomeScreenData()
        {
            var result = await _productService.GetHomeScreenDataAsync();
            return Ok(result);
        }

        // Endpoint MỚI: Tải 20 Biến thể theo PhanLoaiID
        [HttpGet("variants/by-phanloai/{phanLoaiId}")]
        public async Task<IActionResult> GetTop20VariantsByPhanLoai(int phanLoaiId)
        {
            if (phanLoaiId <= 0)
            {
                return BadRequest("Mã phân loại không hợp lệ.");
            }

            var variants = await _productService.GetTop20VariantsByPhanLoaiAsync(phanLoaiId);
            return Ok(variants);
        }

       
    }
}
