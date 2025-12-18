using Microsoft.AspNetCore.Mvc;
using Test_Api_DB_Connect_Stylo_App.DTOs;
using Test_Api_DB_Connect_Stylo_App.Services;

namespace Test_Api_DB_Connect_Stylo_App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _service; 

        public OrderController(OrderService service) => _service = service;

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequestDto request)
        {
            try
            {
                int orderId = await _service.ProcessCheckoutAsync(request);
                return Ok(new { Message = "Thanh toán thành công", OrderID = orderId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
