using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFO.Order.Dtos.OrderManagementModule;
using WFO.Shared.Dtos.Common;

namespace WFO.Order.ApplicationService.OrderManagementModule.Abstracts
{
    public interface IOrderService
    {
        public void CreateOrder(CreateOrderDto input, int CustomerId);
        public void MyOrder(int CustomerId);
        public void ConfirmOrder(int OrderId);
        public void CancelOrder(int OrderId);
        public List<OrderDetailItemDto> GetDetailOrder(int OrderId);
    }
}
