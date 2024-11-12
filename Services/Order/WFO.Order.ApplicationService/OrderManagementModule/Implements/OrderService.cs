using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFO.Order.ApplicationService.Common;
using WFO.Order.ApplicationService.OrderManagementModule.Abstracts;
using WFO.Order.Dtos.OrderManagementModule;
using WFO.Order.Infrastructure;

namespace WFO.Order.ApplicationService.OrderManagementModule.Implements
{
    public class OrderService : OrderServiceBase, IOrderService
    {
        public OrderService(ILogger<OrderService> logger, OrderDbContext dbContext) : base(logger, dbContext)
        {
        }

        public void CancelOrder(int OrderId)
        {
            throw new NotImplementedException();
        }

        public void ConfirmOrder(int OrderId)
        {
            throw new NotImplementedException();
        }

        public void CreateOrder(CreateOrderDto input, int CustomerId)
        {
            throw new NotImplementedException();
        }

        public List<OrderDetailItemDto> GetDetailOrder(int OrderId)
        {
            throw new NotImplementedException();
        }

        public void MyOrder(int CustomerId)
        {
            throw new NotImplementedException();
        }
    }
}
