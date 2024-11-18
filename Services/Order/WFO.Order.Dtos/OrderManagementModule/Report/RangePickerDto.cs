using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFO.Order.Dtos.OrderManagementModule.Report
{
    public class RangePickerDto
    {
        public DateOnly startDate { get; set; }
        public DateOnly endDate { get; set; }
    }
}
