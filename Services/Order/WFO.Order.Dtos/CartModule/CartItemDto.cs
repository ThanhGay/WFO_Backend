using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFO.Order.Dtos.CartModule
{
    public class CartItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public required string ProductName { get; set; }
        public int ProductPrice { get; set; }
        public string? ProductImage { get; set; }
        public required string ProductSize { get; set; }
        public int Quantity { get; set; }
        public string? Note { get; set; }
    }
}
