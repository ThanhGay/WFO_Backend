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

        [HttpGet]
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

        [HttpPost]
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
    }
}
