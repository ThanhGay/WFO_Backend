using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFO.Product.Domain;
using WFO.Product.Dtos.CategoryModule;
using WFO.Shared.Dtos.Common;

namespace WFO.Product.ApplicationService.CategoryModule.Abstracts
{
    public interface ICategoryService
    {
        public void AddCategory(CreateCategoryDto input);
        public PageResultDto<ProdCategory> GetAll(FilterDto input);
    }
}
