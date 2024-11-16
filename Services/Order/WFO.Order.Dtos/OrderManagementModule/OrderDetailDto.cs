using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFO.Order.Dtos.OrderManagementModule
{
    public class OrderDetailDto
    {
        public int Id { get; set; }
        public int Status { get; set; }
        public List<OrderDetailItemDto>? Details { get; set; }
    }
}
