using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFO.Order.Infrastructure;

namespace WFO.Order.ApplicationService.Common
{
    public abstract class OrderServiceBase
    {
        protected readonly ILogger _logger;
        protected readonly OrderDbContext _dbContext;

        protected OrderServiceBase(ILogger logger, OrderDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
    }
}
