using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WFO.Order.ApplicationService.CartModule.Abstracts;
using WFO.Order.ApplicationService.Common;
using WFO.Order.Domain;
using WFO.Order.Dtos.CartModule;
using WFO.Order.Infrastructure;
using WFO.Product.Dtos.ProductManagerModule;
using WFO.Shared.ApplicationService.Product;
using WFO.Shared.Dtos.Common;

namespace WFO.Order.ApplicationService.CartModule.Implements
{
    public class CartService : OrderServiceBase, ICartService
    {
        private readonly IProductInforService _productInforService;

        public CartService(
            ILogger<CartService> logger,
            OrderDbContext dbContext,
            IProductInforService productInforService
        )
            : base(logger, dbContext)
        {
            _productInforService = productInforService;
        }

        public void AddToCart(AddToCartDto input, int customerId)
        {
            var existProduct = _productInforService.HasProduct(input.ProductId);
            var existItem = _dbContext.Carts.FirstOrDefault(c =>
                c.ProductId == input.ProductId && c.CustomerId == customerId
            );
            if (existProduct)
            {
                if (existItem != null)
                {
                    existItem.Quantity += input.Quantity;
                }
                else
                {
                    var newItem = new OrdCart
                    {
                        ProductId = input.ProductId,
                        Quantity = input.Quantity,
                        CustomerId = customerId,
                        Note = input.Note,
                    };

                    _dbContext.Carts.Add(newItem);
                }
                _dbContext.SaveChanges();
            }
            else
            {
                throw new Exception($"Không tồn tại sản phẩm có Id: {input.ProductId}");
            }
        }

        public PageResultDto<CartItemDto> GetMyCart (FilterDto input, int customerId)
        {
            var result = new PageResultDto<CartItemDto>();

            var listProdQuery = _dbContext.Carts.Where(c => c.CustomerId == customerId).ToList();

            var tmpList = new List<ProductDto>();

            foreach (var item in listProdQuery)
            {
                var productItem = _productInforService.GetProduct(item.ProductId);
                tmpList.Add(productItem);
            }

            var finalQuery = listProdQuery.Join(tmpList, cart => cart.ProductId, tmp => tmp.Id, (cart, tmp) => new CartItemDto()
            {
                Id = cart.Id,
                ProductName = tmp.Name,
                ProductSize = tmp.Size,
                ProductId = tmp.Id,
                ProductImage = tmp.Image,
                Note = cart.Note,
                ProductPrice = tmp.Price,
                Quantity = cart.Quantity,
            });

            if (!string.IsNullOrEmpty(input.Keyword))
            {
                finalQuery = finalQuery.Where(s => s.ProductName.ToLower().Contains(input.Keyword.ToLower()));
            }

            int totalItems = finalQuery.Count();

            finalQuery = finalQuery.Skip(input.SkipCount()).Take(input.PageSize);

            result.TotalItem = totalItems;
            result.Items = finalQuery;

            return result;
        }
    }
}
