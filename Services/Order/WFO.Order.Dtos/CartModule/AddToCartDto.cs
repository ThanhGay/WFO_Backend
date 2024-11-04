using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFO.Order.Dtos.CartModule
{
    public class AddToCartDto
    {
        public int ProductId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage ="Số lượng sản phẩm không được nhỏ hơn 1")]
        public int Quantity { get; set; }
        public string? Note { get; set; }
    }
}
