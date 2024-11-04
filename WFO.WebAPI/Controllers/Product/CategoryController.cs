using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WFO.Auth.Dtos.Authorization;
using WFO.Product.ApplicationService.CategoryModule.Abstracts;
using WFO.Product.Dtos.CategoryModule;
using WFO.Shared.Dtos.Common;

namespace WFO.WebAPI.Controllers.Product
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("all")]
        public IActionResult All(FilterDto input)
        {
            try
            {
                return Ok(_categoryService.GetAll(input));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [AuthorizationFilter("Admin")]
        [HttpPost("add")]
        public IActionResult AddCategory(CreateCategoryDto input)
        {
            try
            {
                _categoryService.AddCategory(input);
                return Ok("Thêm danh mục sản phẩm thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
