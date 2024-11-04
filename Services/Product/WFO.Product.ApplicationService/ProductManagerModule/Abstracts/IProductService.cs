using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFO.Shared.Dtos.Common;
using WFO.Product.Dtos.ProductManagerModule;
using WFO.Product.Domain;

namespace WFO.Product.ApplicationService.ProductManagerModule.Abstracts
{
    public interface IProductService
    {
        public Task<ProdProduct> CreateProduct(CreateProductDto input);
        public void DeleteProduct(int id);
        public PageResultDto<ProductDto> GetAll(FilterDto input);
        public ProductDto Get(int id);
        public PageResultDto<ProductDto> GetAllByCategory(FilterDto input, int categoryId);
    }
}
