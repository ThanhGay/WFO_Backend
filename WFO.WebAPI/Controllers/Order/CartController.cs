using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WFO.Auth.Dtos.Authorization;
using WFO.Order.ApplicationService.CartModule.Abstracts;
using WFO.Order.Dtos.CartModule;
using WFO.Shared.ApplicationService.User;
using WFO.Shared.Dtos.Common;

namespace WFO.WebAPI.Controllers.Order
{
    [Route("api/cart")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IUserInforService _userInforService;
        private readonly IHttpContextAccessor _contextAccessor;

        public CartController(
            ICartService cartService,
            IUserInforService userInforService,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _cartService = cartService;
            _userInforService = userInforService;
            _contextAccessor = httpContextAccessor;
        }

        [HttpGet("my-cart")]
        public IActionResult GetMyCart(FilterDto input)
        {
            try
            {
                var customerId = _userInforService.GetCurrentUserId(_contextAccessor);
                return Ok(_cartService.GetMyCart(input, customerId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("add-to-cart")]
        public IActionResult AddToCart(AddToCartDto input)
        {
            try
            {
                var customerId = _userInforService.GetCurrentUserId(_contextAccessor);
                _cartService.AddToCart(input, customerId);
                return Ok($"Thêm sản phẩm cho customerId: {customerId} thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("increase/{id}")]
        public IActionResult IncreaseQuantity(int id)
        {
            try
            {
                var customerId = _userInforService.GetCurrentUserId(_contextAccessor);
                _cartService.IncreaseQuantity(id, customerId);
                return Ok("Đă tăng 1.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("decrease/{id}")]
        public IActionResult DecreaseQuantity(int id)
        {
            try
            {
                var customerId = _userInforService.GetCurrentUserId(_contextAccessor);
                _cartService.DecreaseQuantity(id, customerId);
                return Ok("Đã giảm 1");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteCart(int id)
        {
            try
            {
                var customerId = _userInforService.GetCurrentUserId(_contextAccessor);
                _cartService.RemoveFromCart(id, customerId);
                return Ok("Đã xóa sản phẩm khỏi giỏ hàng của bạn");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
