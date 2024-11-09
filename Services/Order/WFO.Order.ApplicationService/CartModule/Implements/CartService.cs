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
                    existItem.Note = input.Note;
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

        public void DecreaseQuantity(int cartId, int customerId)
        {
            var existCartItem = _dbContext.Carts.FirstOrDefault(c =>
                c.Id == cartId && c.CustomerId == customerId
            );

            if (existCartItem != null)
            {
                if (existCartItem.Quantity > 1)
                {
                    existCartItem.Quantity -= 1;
                }
                else
                {
                    _dbContext.Carts.Remove(existCartItem);
                }

                _dbContext.SaveChanges();
            }
            else
            {
                throw new Exception(
                    $"Không tìm thấy giỏ hàng có Id \"{cartId}\" hoặc bạn không có quyền thay đổi số lượng."
                );
            }
        }

        public void IncreaseQuantity(int cartId, int customerId)
        {
            var existCartItem = _dbContext.Carts.FirstOrDefault(c =>
                c.Id == cartId && c.CustomerId == customerId
            );

            if (existCartItem != null)
            {
                existCartItem.Quantity += 1;
                _dbContext.SaveChanges();
            }
            else
            {
                throw new Exception(
                    $"Không tìm thấy giỏ hàng có Id \"{cartId}\" hoặc bạn không có quyền thay đổi số lượng."
                );
            }
        }

        public void RemoveFromCart(int cartId, int customerId)
        {
            var existCartItem = _dbContext.Carts.FirstOrDefault(c =>
                c.Id == cartId && c.CustomerId == customerId
            );

            if (existCartItem != null)
            {
                _dbContext.Carts.Remove(existCartItem);
                _dbContext.SaveChanges();
            }
            else
            {
                throw new Exception(
                    $"Không tìm thấy giỏ hàng có Id \"{cartId}\" hoặc bạn không có quyền xóa nó."
                );
            }
        }

        public PageResultDto<CartItemDto> GetMyCart(FilterDto input, int customerId)
        {
            var result = new PageResultDto<CartItemDto>();

            //get record in Cart table with MyCustomerId
            var listProdQuery = _dbContext.Carts.Where(c => c.CustomerId == customerId).ToList();

            // save information of product into list
            var tmpList = new List<ProductDto>();
            foreach (var item in listProdQuery)
            {
                var productItem = _productInforService.GetProduct(item.ProductId);
                tmpList.Add(productItem);
            }

            // join list
            var finalQuery = listProdQuery.Join(
                tmpList,
                cart => cart.ProductId,
                tmp => tmp.Id,
                (cart, tmp) =>
                    new CartItemDto()
                    {
                        Id = cart.Id,
                        ProductName = tmp.Name,
                        ProductSize = tmp.Size,
                        ProductId = tmp.Id,
                        ProductImage = tmp.Image,
                        Note = cart.Note,
                        ProductPrice = tmp.Price,
                        Quantity = cart.Quantity,
                    }
            );

            if (!string.IsNullOrEmpty(input.Keyword))
            {
                finalQuery = finalQuery.Where(s =>
                    s.ProductName.ToLower().Contains(input.Keyword.ToLower())
                );
            }

            int totalItems = finalQuery.Count();

            finalQuery = finalQuery.Skip(input.SkipCount()).Take(input.PageSize);

            result.TotalItem = totalItems;
            result.Items = finalQuery;

            return result;
        }
    }
}
