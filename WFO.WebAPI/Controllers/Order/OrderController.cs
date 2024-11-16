using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WFO.Auth.Dtos.Authorization;
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

        public OrderController(
            IOrderService orderService,
            IHttpContextAccessor contextAccessor,
            IUserInforService userInforService
        )
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

        [HttpDelete("cancel")]
        public IActionResult DeleteOrder([FromForm] int orderId)
        {
            try
            {
                var customerId = _userInforService.GetCurrentUserId(_contextAccessor);
                _orderService.CancelOrder(orderId, customerId);
                return Ok("Hủy đơn hàng thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("confirm")]
        [AuthorizationFilter("Admin")]
        public IActionResult ConfirmOrder([FromForm] int orderId)
        {
            try
            {
                _orderService.ConfirmOrder(orderId);
                return Ok("Admin đã xác nhận đơn hàng");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("shipping")]
        [AuthorizationFilter("Admin")]
        public IActionResult TransferOrderToCarrier([FromForm] int orderId)
        {
            try
            {
                _orderService.TransferToCarrier(orderId);
                return Ok("Đơn hàng đã được giao cho shipper");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("received")]
        [AuthorizationFilter("Customer")]
        public IActionResult CustomerReceive([FromForm] int orderId)
        {
            try
            {
                var customerId = _userInforService.GetCurrentUserId(_contextAccessor);
                _orderService.CustomerConfirmReceive(orderId, customerId);
                return Ok("Bạn đã nhận hàng");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("finish")]
        public IActionResult FinishOrder([FromForm] int orderId)
        {
            try
            {
                _orderService.SucceededOrder(orderId);
                return Ok("Đơn hàng đã được hoàn thành");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("my-orders")]
        public IActionResult GetMyOrders()
        {
            try
            {
                var customerId = _userInforService.GetCurrentUserId(_contextAccessor);
                return Ok(_orderService.MyOrder(customerId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("detail/{orderId}")]
        public IActionResult GetDetailOrder(int orderId)
        {
            try
            {
                var customerId = _userInforService.GetCurrentUserId(_contextAccessor);
                return Ok(_orderService.GetDetailOrder(orderId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
