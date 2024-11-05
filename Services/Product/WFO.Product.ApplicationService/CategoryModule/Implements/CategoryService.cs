using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WFO.Product.ApplicationService.CategoryModule.Abstracts;
using WFO.Product.ApplicationService.Common;
using WFO.Product.ApplicationService.ProductManagerModule.Implements;
using WFO.Product.Domain;
using WFO.Product.Dtos.CategoryModule;
using WFO.Product.Infrastructure;
using WFO.Shared.Dtos.Common;

namespace WFO.Product.ApplicationService.CategoryModule.Implements
{
    public class CategoryService : ProductServiceBase, ICategoryService
    {
        public CategoryService(ILogger<CategoryService> logger, ProductDbContext dbContext)
            : base(logger, dbContext) { }

        public async Task<ProdCategory> AddCategory(CreateCategoryDto input)
        {
            var existCategory = _dbContext.Categories.Any(c => c.Name == input.Name);
            if (existCategory)
            {
                throw new Exception($"Đã tồn tại phân loại {input.Name}");
            }
            else
            {
                string? createdImageName = null;

                if (input.ImageFile != null)
                {
                    string[] allowedFileExtentions = [".jpg", ".jpeg", ".png"];

                    var ext = Path.GetExtension(input.ImageFile.FileName);
                    if (!allowedFileExtentions.Contains(ext))
                    {
                        throw new ArgumentException(
                            $"Only {string.Join(",", allowedFileExtentions)} are allowed."
                        );
                    }

                    if (input.ImageFile.Length > 0)
                    {
                        var path = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot",
                            "Images",
                            input.ImageFile.FileName
                        );
                        using (var stream = System.IO.File.Create(path))
                        {
                            await input.ImageFile.CopyToAsync(stream);
                        }
                        ;

                        createdImageName = "/images/" + input.ImageFile.FileName;
                    }
                }
                var newCategory = new ProdCategory
                {
                    Name = input.Name,
                    CreatedDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Image = createdImageName,
                };

                _dbContext.Categories.Add(newCategory);
                _dbContext.SaveChanges();

                return newCategory;
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
                query = query.Where(c => c.Name.ToLower().Contains(input.Keyword.ToLower()));
            }

            int totalItems = query.Count();
            query = query.Skip(input.SkipCount()).Take(input.PageSize);

            result.Items = query;
            result.TotalItem = totalItems;

            return result;
        }

        public async Task<ProdCategory> UpdateCategory(UpdateCategoryDto input)
        {
            var existCategory = await _dbContext.Categories.FirstOrDefaultAsync(c =>
                c.Id == input.Id
            );
            var existCategoryName = await _dbContext.Categories.AnyAsync(c =>
                c.Name == input.Name && c.Id != input.Id
            );
            if (!existCategoryName)
            {
                if (existCategory != null)
                {
                    string? createdImageName = null;

                    if (input.ImageFile != null)
                    {
                        string[] allowedFileExtentions = [".jpg", ".jpeg", ".png"];

                        var ext = Path.GetExtension(input.ImageFile.FileName);
                        if (!allowedFileExtentions.Contains(ext))
                        {
                            throw new ArgumentException(
                                $"Only {string.Join(",", allowedFileExtentions)} are allowed."
                            );
                        }

                        if (input.ImageFile.Length > 0)
                        {
                            var path = Path.Combine(
                                Directory.GetCurrentDirectory(),
                                "wwwroot",
                                "Images",
                                input.ImageFile.FileName
                            );
                            using (var stream = System.IO.File.Create(path))
                            {
                                await input.ImageFile.CopyToAsync(stream);
                            }
                            ;

                            createdImageName = "/images/" + input.ImageFile.FileName;
                        }
                    }
                    else
                    {
                        createdImageName = input.Image;
                    }

                    existCategory.Name = input.Name;
                    existCategory.UpdateDate = DateTime.Now;
                    existCategory.Image = createdImageName;

                    _dbContext.SaveChanges();

                    return existCategory;
                }
                else
                {
                    throw new Exception($"Không tìm thấy thể loại có Id: {input.Id}");
                }
            }
            else
            {
                throw new Exception($"Đã tồn tại thể loại tên là \"{input.Name}\"");
            }
        }

        public void DeleteCategory(int categoryId)
        {
            var existCategory = _dbContext.Categories.FirstOrDefault(c => c.Id == categoryId);

            if (existCategory != null)
            {
                var listNeedRemove = _dbContext
                    .ProductCategories.Where(c => c.CategoryId == categoryId)
                    .Select(pc => new ProdProductCategory
                    {
                        Id = pc.Id,
                        CategoryId = categoryId,
                        ProductId = pc.ProductId,
                    });

                foreach (var item in listNeedRemove)
                {
                    _dbContext.ProductCategories.Remove(item);
                }

                _dbContext.Categories.Remove(existCategory);
                _dbContext.SaveChanges();
            }
            else
            {
                throw new Exception($"Không tìm thấy thể loại có Id: {categoryId}");
            }
        }
    }
}
