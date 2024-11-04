using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFO.Product.Dtos.ProductManagerModule;

namespace WFO.Shared.ApplicationService.Product
{
    public interface IProductInforService
    {
        public ProductDto GetProduct(int id);
        public bool HasProduct(int id);
    }
}
