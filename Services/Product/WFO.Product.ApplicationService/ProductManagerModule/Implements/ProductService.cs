﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WFO.Product.ApplicationService.Common;
using WFO.Product.ApplicationService.ProductManagerModule.Abstracts;
using WFO.Product.Domain;
using WFO.Product.Dtos.ProductManagerModule;
using WFO.Product.Infrastructure;
using WFO.Shared.Dtos.Common;

namespace WFO.Product.ApplicationService.ProductManagerModule.Implements
{
    public class ProductService : ProductServiceBase, IProductService
    {
        public ProductService(ILogger<ProductService> logger, ProductDbContext dbContext)
            : base(logger, dbContext) { }

        public async Task<ProdProduct> CreateProduct(CreateProductDto input)
        {
            var existProduct = _dbContext.Products.Any(prod =>
                prod.Name == input.Name && prod.Size == input.Size
            );

            if (!existProduct)
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

                var newProduct = new ProdProduct()
                {
                    Name = input.Name,
                    Price = input.Price,
                    Size = input.Size,
                    Description = input.Description,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Image = createdImageName,
                    IsAvailable = true,
                };

                _dbContext.Products.Add(newProduct);
                _dbContext.SaveChanges();

                if (input.CategoryId != null)
                {
                    var existCategory = _dbContext.Categories.Any(c => c.Id == input.CategoryId);
                    if (!existCategory)
                    {
                        throw new Exception(
                            $"Thêm sản phẩm thành công, nhưng chưa tồn tại thể loại có Id \"{input.CategoryId}\""
                        );
                    }
                    else
                    {
                        _dbContext.ProductCategories.Add(
                            new ProdProductCategory()
                            {
                                CategoryId = (int)input.CategoryId,
                                ProductId = newProduct.Id,
                            }
                        );
                    }
                    _dbContext.SaveChanges();
                }

                return newProduct;
            }
            else
            {
                throw new Exception($"Đã tồn tại món ăn có tên {input.Name} size {input.Size}.");
            }
        }

        public void DeleteProduct(int id)
        {
            var existProduct = _dbContext.Products.FirstOrDefault(c => c.Id == id);
            if (existProduct != null)
            {
                existProduct.IsAvailable = false;

                _dbContext.Products.Update(existProduct);
                _dbContext.SaveChanges();
            }
            else
            {
                throw new Exception($"Không tồn tại sản phẩm có Id \"{id}\"");
            }
        }

        public ProductDto Get(int id)
        {
            var existProduct = _dbContext.Products.Any(c => c.Id == id);
            var existProdCate = _dbContext.ProductCategories.Any(c => c.ProductId == id);
            if (!existProduct)
            {
                throw new Exception(
                    $"Không tồn tại sản phẩm có Id: {id} hoặc sản phẩm đã ngừng kinh doanh"
                );
            }
            else if (!existProdCate)
            {
                var result = _dbContext
                    .Products.Where(c => c.Id == id)
                    .Select(prod => new ProductDto
                    {
                        Name = prod.Name,
                        Size = prod.Size,
                        Id = prod.Id,
                        Description = prod.Description,
                        Image = prod.Image,
                        Price = prod.Price,
                        CategoryId = null,
                        CategoryName = null,
                        IsAvailable = prod.IsAvailable,
                    })
                    .ToList();

                return result[0];
            }
            else
            {
                var result = _dbContext
                    .ProductCategories.Where(pc => pc.ProductId == id)
                    .Join(
                        _dbContext.Products,
                        (pc => pc.ProductId),
                        (prod => prod.Id),
                        (pc, prod) =>
                            new
                            {
                                prod.Name,
                                prod.Size,
                                prod.Id,
                                prod.Description,
                                prod.Image,
                                prod.Price,
                                pc.CategoryId,
                                prod.IsAvailable,
                                CategoryName = _dbContext
                                    .Categories.Where(c => c.Id == pc.CategoryId)
                                    .Select(c => c.Name)
                                    .ToList(),
                            }
                    )
                    .Select(p => new ProductDto
                    {
                        Name = p.Name,
                        Size = p.Size,
                        Id = p.Id,
                        Description = p.Description,
                        Image = p.Image,
                        Price = p.Price,
                        IsAvailable= p.IsAvailable,
                        CategoryId = p.CategoryId,
                        CategoryName = p.CategoryName[0],
                    })
                    .ToList();

                return result[0];
            }
        }

        public PageResultDto<ProductDto> GetAll(FilterDto input)
        {
            var result = new PageResultDto<ProductDto>();

            var query =
                from p in _dbContext.Products
                join pc in _dbContext.ProductCategories
                    on p.Id equals pc.ProductId
                    into productCategories
                from pc in productCategories.DefaultIfEmpty()
                join c in _dbContext.Categories on pc.CategoryId equals c.Id into categories
                from c in categories.DefaultIfEmpty()
                where p.IsAvailable == true
                select new ProductDto
                {
                    Name = p.Name,
                    Size = p.Size,
                    Id = p.Id,
                    Description = p.Description,
                    Image = p.Image,
                    Price = p.Price,
                    CategoryId = pc.CategoryId,
                    CategoryName = c.Name,
                };

            if (!string.IsNullOrEmpty(input.Keyword))
            {
                query = query.Where(prod => prod.Name.ToLower().Contains(input.Keyword.ToLower()));
            }

            var totalItems = query.Count();

            query = query.Skip(input.SkipCount()).Take(input.PageSize);

            result.TotalItem = totalItems;
            result.Items = query.ToList();

            return result;
        }

        public PageResultDto<ProductDto> GetAllByAdmin(FilterDto input)
        {
            var result = new PageResultDto<ProductDto>();

            var query =
                from p in _dbContext.Products
                join pc in _dbContext.ProductCategories
                    on p.Id equals pc.ProductId
                    into productCategories
                from pc in productCategories.DefaultIfEmpty()
                join c in _dbContext.Categories on pc.CategoryId equals c.Id into categories
                from c in categories.DefaultIfEmpty()
                select new ProductDto
                {
                    Name = p.Name,
                    Size = p.Size,
                    Id = p.Id,
                    Description = p.Description,
                    Image = p.Image,
                    Price = p.Price,
                    CategoryId = pc.CategoryId,
                    CategoryName = c.Name,
                    IsAvailable = p.IsAvailable,
                };

            if (!string.IsNullOrEmpty(input.Keyword))
            {
                query = query.Where(prod => prod.Name.ToLower().Contains(input.Keyword.ToLower()));
            }

            var totalItems = query.Count();

            query = query.Skip(input.SkipCount()).Take(input.PageSize);

            result.TotalItem = totalItems;
            result.Items = query.ToList();

            return result;
        }

        public PageResultDto<ProductDto> GetAllByCategory(FilterDto input, int categoryId)
        {
            var result = new PageResultDto<ProductDto>();

            var query = _dbContext
                .ProductCategories.Where(pc => pc.CategoryId == categoryId)
                .Join(
                    _dbContext.Products,
                    (pc => pc.ProductId),
                    (prod => prod.Id),
                    (pc, prod) =>
                        new
                        {
                            prod.Name,
                            prod.Size,
                            prod.Id,
                            prod.Description,
                            prod.Image,
                            prod.Price,
                            pc.CategoryId,
                            CategoryName = _dbContext
                                .Categories.Where(c => c.Id == pc.CategoryId)
                                .Select(c => c.Name)
                                .ToList(),
                            prod.IsAvailable,
                        }
                )
                .Select(p => new ProductDto
                {
                    Name = p.Name,
                    Size = p.Size,
                    Id = p.Id,
                    Description = p.Description,
                    Image = p.Image,
                    Price = p.Price,
                    CategoryId = p.CategoryId,
                    CategoryName = p.CategoryName[0],
                    IsAvailable = p.IsAvailable,
                });

            if (!string.IsNullOrEmpty(input.Keyword))
            {
                query = query.Where(prod => prod.Name.ToLower().Contains(input.Keyword.ToLower()));
            }

            var totalItems = query.Count();

            query = query.Skip(input.SkipCount()).Take(input.PageSize);

            result.TotalItem = totalItems;
            result.Items = query.ToList();

            return result;
        }

        public async Task<ProdProduct> UpdateProduct(UpdateProductDto input)
        {
            var existProuduct = await _dbContext.Products.FirstOrDefaultAsync(prod =>
                prod.Id == input.Id
            );
            var existProductNameSize = await _dbContext.Products.AnyAsync(prod =>
                prod.Id != input.Id && prod.Name == input.Name && prod.Size == input.Size
            );
            var existCategory = await _dbContext.Categories.AnyAsync(c => c.Id == input.CategoryId);
            var existProductInCategory = await _dbContext.ProductCategories.FirstOrDefaultAsync(
                pc => pc.ProductId == input.Id
            );

            if (!existProductNameSize)
            {
                if (existProuduct != null)
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

                    existProuduct.Name = input.Name;
                    existProuduct.Description = input.Description;
                    existProuduct.Price = input.Price;
                    existProuduct.Size = input.Size;
                    existProuduct.UpdateDate = DateTime.Now;
                    existProuduct.Image = createdImageName;

                    if (input.CategoryId != null && existCategory)
                    {
                        if (existProductInCategory != null)
                        {
                            existProductInCategory.CategoryId = (int)input.CategoryId;
                        }
                        else
                        {
                            _dbContext.ProductCategories.Add(
                                new ProdProductCategory
                                {
                                    CategoryId = (int)input.CategoryId,
                                    ProductId = input.Id,
                                }
                            );
                        }
                    }

                    _dbContext.SaveChanges();

                    return existProuduct;
                }
                else
                {
                    throw new Exception($"Không tồn tại sản phẩm có Id {input.Id}");
                }
            }
            else
            {
                throw new Exception($"Đã tồn tại sản phẩm có tên {input.Name} size {input.Size}");
            }
        }
    }
}
