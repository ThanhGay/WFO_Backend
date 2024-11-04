using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WFO.Product.ApplicationService.CategoryModule.Abstracts;
using WFO.Product.ApplicationService.CategoryModule.Implements;
using WFO.Product.ApplicationService.ProductManagerModule.Abstracts;
using WFO.Product.ApplicationService.ProductManagerModule.Implements;
using WFO.Product.Infrastructure;
using WFO.Shared.ApplicationService.Product;
using WFO.Shared.Constant.Database;

namespace WFO.Product.ApplicationService.Startup
{
    public static class ProductStartup
    {
        /// <summary>
        /// extension method to configure the product database
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="assemblyName"></param>
        public static void ConfigureProduct(
            this WebApplicationBuilder builder,
            string? assemblyName
        )
        {
            builder.Services.AddDbContext<ProductDbContext>(
                options =>
                {
                    options.UseSqlServer(
                        builder.Configuration.GetConnectionString("Default"),
                        options =>
                        {
                            options.MigrationsAssembly(assemblyName);
                            options.MigrationsHistoryTable(
                                DbSchema.TableMigrationsHistory,
                                DbSchema.Product
                            );
                        }
                    );
                },
                ServiceLifetime.Scoped
            );

            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IProductInforService, ProductInforService>();
        }
    }
}
