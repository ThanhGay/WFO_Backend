using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WFO.Auth.ApplicationService.UserModule.Abstracts;
using WFO.Auth.Dtos.UserModule;
using WFO.Shared.ApplicationService.User;

namespace WFO.WebAPI.Controllers.Auth
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserInforService _userInforService;

        public UserController(
            IUserService userService,
            IConfiguration configuration,
            IHttpContextAccessor contextAccessor,
            IUserInforService userInforService
        )
        {
            _userService = userService;
            _userInforService = userInforService;
            _configuration = configuration;
            _contextAccessor = contextAccessor;
        }

        /// <summary>
        /// Lấy tất cả danh sách tài khoản
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        public IActionResult All()
        {
            try
            {
                return Ok(_userService.GetAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Thêm account customer
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("signup")]
        public IActionResult CreateUser(CreateUserDto input)
        {
            try
            {
                _userService.CreateUser(input);
                return Ok("Tạo tài khoản thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(LoginDto input)
        {
            try
            {
                return Ok(_userService.Login(input));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Cập nhật thông tin user
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("update")]
        public IActionResult Update(UpdateInforUserDto input)
        {
            try
            {
                var customerId = _userInforService.GetCurrentUserId(_contextAccessor);
                _userService.UpdateUser(input, customerId);
                return Ok("Cập nhật thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
