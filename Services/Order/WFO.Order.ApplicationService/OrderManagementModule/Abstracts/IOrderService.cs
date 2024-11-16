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
        public PageResultDto<ListOrdersOfCustomerDto> MyOrder(int CustomerId);
        public OrderDetailDto GetDetailOrder(int OrderId);
        public void CreateOrder(CreateOrderDto input, int CustomerId);
        public void ConfirmOrder(int OrderId);
        public void TransferToCarrier(int OrderId);
        public void CustomerConfirmReceive(int OrderId, int CustomerId);
        public void SucceededOrder(int OrderId);
        public void CancelOrder(int OrderId, int CustomerId);
    }
}
