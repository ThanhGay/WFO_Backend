using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WFO.Product.ApplicationService.CategoryModule.Abstracts;
using WFO.Product.ApplicationService.Common;
using WFO.Product.ApplicationService.ProductManagerModule.Implements;
using WFO.Product.Domain;
using WFO.Product.Dtos.CategoryModule;
using WFO.Shared.Dtos.Common;
using WFO.Product.Infrastructure;

namespace WFO.Product.ApplicationService.CategoryModule.Implements
{
    public class CategoryService : ProductServiceBase, ICategoryService
    {
        public CategoryService(ILogger<CategoryService> logger, ProductDbContext dbContext)
            : base(logger, dbContext) { }

        public void AddCategory(CreateCategoryDto input)
        {
            var existCategory = _dbContext.Categories.Any(c => c.Name == input.Name);
            if (existCategory)
            {
                throw new Exception($"Đã tồn tại phân loại {input.Name}");
            }
            else
            {
                var newCategory = new ProdCategory
                {
                    Name = input.Name,
                    CreatedDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Image = input.Image,
                };

                _dbContext.Categories.Add(newCategory);
                _dbContext.SaveChanges();
            }
        }

        public PageResultDto<ProdCategory> GetAll(FilterDto input)
        {
            var result = new PageResultDto<ProdCategory>();

            var query = _dbContext.Categories.Select(c => new ProdCategory
            {
                Name = c.Name,
                Id = c.Id,
                Image = c.Image,
            });

            if (!string.IsNullOrEmpty(input.Keyword))
            {
                query = query.Where(c =>
                    c.Name.ToLower().Contains(input.Keyword.ToLower())
                );
            }

            int totalItems = query.Count();
            query = query.Skip(input.SkipCount()).Take(input.PageSize);

            result.Items = query;
            result.TotalItem = totalItems;

            return result;
        }
    }
}
