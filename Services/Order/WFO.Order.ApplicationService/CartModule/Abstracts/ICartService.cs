using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFO.Order.Domain;
using WFO.Order.Dtos.CartModule;
using WFO.Shared.Dtos.Common;

namespace WFO.Order.ApplicationService.CartModule.Abstracts
{
    public interface ICartService
    {
        public void AddToCart(AddToCartDto input, int customerId);
        public PageResultDto<CartItemDto> GetMyCart(FilterDto input, int customerId);
        public void IncreaseQuantity(int id, int customerId);
        public void DecreaseQuantity(int id, int customerId);
        public void RemoveFromCart(int id, int customerId);
        public bool HasItemInCart(int id, int customerId);
    }
}
