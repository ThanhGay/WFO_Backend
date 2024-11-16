using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFO.Order.Dtos.OrderManagementModule
{
    public class ListOrdersOfCustomerDto
    {
        public int Id { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? CanceledAt { get; set; }
        public int Status { get; set; }
        public int ProductCount { get; set; }
        public int TotalPrice { get; set; }
    }
}
