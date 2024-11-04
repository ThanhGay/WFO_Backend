using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFO.Order.ApplicationService.CartModule.Abstracts;
using WFO.Order.ApplicationService.CartModule.Implements;
using WFO.Order.Infrastructure;
using WFO.Shared.Constant.Database;

namespace WFO.Order.ApplicationService.Startup
{
    public static class OrderStartUp
    {
        /// <summary>
        /// extension method to configure the order database
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="assemblyName"></param>
        public static void ConfigureOrder(this WebApplicationBuilder builder, string? assemblyName)
        {
            builder.Services.AddDbContext<OrderDbContext>(
               options =>
               {
                   options.UseSqlServer(
                       builder.Configuration.GetConnectionString("Default"),
                       options =>
                       {
                           options.MigrationsAssembly(assemblyName);
                           options.MigrationsHistoryTable(
                               DbSchema.TableMigrationsHistory,
                               DbSchema.Order
                           );
                       }
                   );
               },
               ServiceLifetime.Scoped
           );

            builder.Services.AddScoped<ICartService, CartService>();
        }
    }
}
