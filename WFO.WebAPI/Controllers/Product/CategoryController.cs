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
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [AllowAnonymous]
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

        [AuthorizationFilter("Admin")]
        [HttpPost("add")]
        public async Task<IActionResult> AddCategory([FromForm] CreateCategoryDto input)
        {
            try
            {
                var newCategory = await _categoryService.AddCategory(input);
                return Ok(newCategory);
                //return Ok("Thêm danh mục sản phẩm thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AuthorizationFilter("Admin")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateCategory([FromForm] UpdateCategoryDto input)
        {
            try
            {
                var updatedCategory = await _categoryService.UpdateCategory(input);
                return Ok(updatedCategory);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AuthorizationFilter("Admin")]
        [HttpDelete("delete")]
        public IActionResult DeleteCategory(int categoryId)
        {
            try
            {
                _categoryService.DeleteCategory(categoryId);
                return Ok("Xóa thể loại thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
