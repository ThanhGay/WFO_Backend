using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WFO.Order.ApplicationService.OrderManagementModule.Abstracts;
using WFO.Order.Dtos.OrderManagementModule;
using WFO.Shared.ApplicationService.User;

namespace WFO.WebAPI.Controllers.Order
{
    [Route("api/order")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserInforService _userInforService;

        public OrderController(IOrderService orderService, IHttpContextAccessor contextAccessor, IUserInforService userInforService)
        {
            _orderService = orderService;
            _contextAccessor = contextAccessor;
            _userInforService = userInforService;
        }

        [HttpPost("create")]
        public IActionResult CreateOrder(CreateOrderDto input)
        {
            try
            {
                var customerId = _userInforService.GetCurrentUserId(_contextAccessor);
                _orderService.CreateOrder(input, customerId);
                return Ok("Đặt hàng thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
