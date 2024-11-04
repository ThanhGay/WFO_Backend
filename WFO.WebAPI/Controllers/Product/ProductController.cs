using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WFO.Auth.Dtos.Authorization;
using WFO.Product.ApplicationService.ProductManagerModule.Abstracts;
using WFO.Shared.Dtos.Common;
using WFO.Product.Dtos.ProductManagerModule;


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
        public async Task<IActionResult> Create([FromForm]CreateProductDto input)
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
                return Ok(_productService.Get(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

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
