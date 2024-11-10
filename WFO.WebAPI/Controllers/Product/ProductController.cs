using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WFO.Auth.Dtos.Authorization;
using WFO.Product.ApplicationService.ProductManagerModule.Abstracts;
using WFO.Product.Dtos.ProductManagerModule;
using WFO.Shared.Dtos.Common;

namespace WFO.WebAPI.Controllers.Product
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [Authorize]
        [AuthorizationFilter("Admin")]
        [HttpPost("add")]
        public async Task<IActionResult> Create([FromForm] CreateProductDto input)
        {
            try
            {
                var createdProduct = await _productService.CreateProduct(input);
                return Ok(createdProduct);
                //return Ok("Thêm sản phẩm thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [AuthorizationFilter("Admin")]
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] UpdateProductDto input)
        {
            try
            {
                var updatedProduct = await _productService.UpdateProduct(input);
                return Ok(updatedProduct);
                //return Ok("Cập nhật sản phẩm thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("all")]
        public IActionResult All(FilterDto input)
        {
            try
            {
                return Ok(_productService.GetAll(input));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [AuthorizationFilter("Admin")]
        [HttpGet("list-product")]
        public IActionResult ListProduct(FilterDto input)
        {
            try
            {
                return Ok(_productService.GetAllByAdmin(input));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("all/{categoryId}")]
        public IActionResult AllByCategory(FilterDto input, int categoryId)
        {
            try
            {
                return Ok(_productService.GetAllByCategory(input, categoryId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get/{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var foundProduct = _productService.Get(id);
                return Ok(foundProduct);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [AuthorizationFilter("Admin")]
        [HttpDelete("delete")]
        public IActionResult Delete(int id)
        {
            try
            {
                _productService.DeleteProduct(id);
                return Ok("Xóa sản phầm thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
