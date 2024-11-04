using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFO.Order.Domain;

namespace WFO.Order.Infrastructure
{
    public class OrderDbContext: DbContext
    {
        public DbSet<OrdCart> Carts { get; set; }

        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
