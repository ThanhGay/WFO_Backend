using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFO.Product.Infrastructure;

namespace WFO.Product.ApplicationService.Common
{
    public abstract class ProductServiceBase
    {
        protected readonly ILogger _logger;
        protected readonly ProductDbContext _dbContext;

        protected ProductServiceBase(ILogger logger, ProductDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
    }
}
