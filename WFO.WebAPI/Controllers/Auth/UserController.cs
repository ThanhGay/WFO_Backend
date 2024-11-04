using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WFO.Auth.ApplicationService.UserModule.Abstracts;
using WFO.Auth.Dtos.UserModule;

namespace WFO.WebAPI.Controllers.Auth
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IUserService _userService;

        public UserController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        /// <summary>
        /// Lấy tất cả danh sách tài khoản
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        public IActionResult All()
        {
            return Ok(_userService.GetAll());
        }

        /// <summary>
        /// Thêm account customer
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("signup")]
        public IActionResult CreateUser(CreateUserDto input)
        {
            _userService.CreateUser(input);
            return Ok("Tạo tài khoản thành công");
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
            return Ok(_userService.Login(input));
        }
    }
}
