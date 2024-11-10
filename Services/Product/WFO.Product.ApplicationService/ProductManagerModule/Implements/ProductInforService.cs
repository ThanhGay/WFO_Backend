using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WFO.Product.ApplicationService.Common;
using WFO.Product.Dtos.ProductManagerModule;
using WFO.Product.Infrastructure;
using WFO.Shared.ApplicationService.Product;

namespace WFO.Product.ApplicationService.ProductManagerModule.Implements
{
    public class ProductInforService : ProductServiceBase, IProductInforService
    {
        public ProductInforService(ILogger<ProductInforService> logger, ProductDbContext dbContext)
            : base(logger, dbContext) { }

        public bool HasProduct(int id)
        {
            return _dbContext.Products.Any(prod => prod.Id == id && prod.IsAvailable == true);
        }

        public ProductDto GetProduct(int id)
        {
            var existProduct = _dbContext.Products.Any(c => c.Id == id);
            var existProdCate = _dbContext.ProductCategories.Any(c => c.ProductId == id);
            if (!existProduct)
            {
                throw new Exception($"Không tồn tại sản phẩm có Id: {id}");
            }
            else if (!existProdCate)
            {
                var result = _dbContext
                    .Products.Where(c => c.Id == id)
                    .Select(prod => new ProductDto
                    {
                        Name = prod.Name,
                        Size = prod.Size,
                        Id = prod.Id,
                        Description = prod.Description,
                        Image = prod.Image,
                        Price = prod.Price,
                        CategoryId = null,
                        CategoryName = null,
                        IsAvailable = prod.IsAvailable,
                    })
                    .ToList();

                return result[0];
            }
            else
            {
                var result = _dbContext
                    .ProductCategories.Where(pc => pc.ProductId == id)
                    .Join(
                        _dbContext.Products,
                        (pc => pc.ProductId),
                        (prod => prod.Id),
                        (pc, prod) =>
                            new
                            {
                                prod.Name,
                                prod.Size,
                                prod.Id,
                                prod.Description,
                                prod.Image,
                                prod.Price,
                                prod.IsAvailable,
                                pc.CategoryId,
                                CategoryName = _dbContext
                                    .Categories.Where(c => c.Id == pc.CategoryId)
                                    .Select(c => c.Name)
                                    .ToList(),
                            }
                    )
                    .Select(p => new ProductDto
                    {
                        Name = p.Name,
                        Size = p.Size,
                        Id = p.Id,
                        Description = p.Description,
                        Image = p.Image,
                        Price = p.Price,
                        IsAvailable = p.IsAvailable,
                        CategoryId = p.CategoryId,
                        CategoryName = p.CategoryName[0],
                    })
                    .ToList();

                return result[0];
            }
        }
    }
}
