using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFO.Product.Dtos.ProductManagerModule
{
    public class UpdateProductDto
    {
        public int Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Tên sản phẩm không được để trống")]
        public required string Name { get; set; }
        public string? Description { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Giá của sản phẩm không được bé hơn 0")]
        public int Price { get; set; }
        public string?  Image { get; set; }
        public IFormFile? ImageFile { get; set; }
        [AllowedValues(["S", "M", "L"], ErrorMessage = "Size của sản phẩm chỉ bao gồm S, M, L")]
        public required string Size { get; set; }
        public int? CategoryId { get; set; }
    }
}
