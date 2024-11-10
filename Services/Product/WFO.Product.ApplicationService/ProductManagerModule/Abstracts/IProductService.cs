using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFO.Product.Domain;
using WFO.Product.Dtos.ProductManagerModule;
using WFO.Shared.Dtos.Common;

namespace WFO.Product.ApplicationService.ProductManagerModule.Abstracts
{
    public interface IProductService
    {
        public Task<ProdProduct> CreateProduct(CreateProductDto input);
        public Task<ProdProduct> UpdateProduct(UpdateProductDto input);
        public void DeleteProduct(int id);
        public PageResultDto<ProductDto> GetAll(FilterDto input);
        public PageResultDto<ProductDto> GetAllByAdmin(FilterDto input);
        public ProductDto Get(int id);
        public PageResultDto<ProductDto> GetAllByCategory(FilterDto input, int categoryId);
    }
}
