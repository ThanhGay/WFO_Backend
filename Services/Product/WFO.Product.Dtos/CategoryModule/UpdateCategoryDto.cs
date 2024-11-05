using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFO.Product.Dtos.CategoryModule
{
    public class UpdateCategoryDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Image { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
