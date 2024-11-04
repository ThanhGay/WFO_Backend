using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFO.Auth.ApplicationService.UserModule.Abstracts;
using WFO.Auth.ApplicationService.UserModule.Implements;
using WFO.Auth.Infrastructure;
using WFO.Shared.ApplicationService.User;
using WFO.Shared.Constant.Database;

namespace WFO.Auth.ApplicationService.Startup
{
    public static class AuthStartup
    {
        /// <summary>
        /// extension method to configure the auth database
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="assemblyName"></param>
        public static void ConfigureAuth(this WebApplicationBuilder builder, string? assemblyName)
        {
            builder.Services.AddDbContext<AuthDbContext>(
                options =>
                {
                    options.UseSqlServer(
                        builder.Configuration.GetConnectionString("Default"),
                        options =>
                        {
                            options.MigrationsAssembly(assemblyName);
                            options.MigrationsHistoryTable(
                                DbSchema.TableMigrationsHistory,
                                DbSchema.Auth
                            );
                        }
                    );
                },
                ServiceLifetime.Scoped
            );

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IUserInforService, UserInforService>();
        }
    }
}
