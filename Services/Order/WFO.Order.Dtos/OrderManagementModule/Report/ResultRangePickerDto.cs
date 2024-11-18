using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFO.Order.Dtos.OrderManagementModule.Report
{
    public class ResultRangePickerDto
    {
        public DateOnly Date {  get; set; }
        public int Amount { get; set; }
    }
}
