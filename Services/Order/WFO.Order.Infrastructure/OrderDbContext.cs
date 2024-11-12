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
        public DbSet<OrdOrder> Orders { get; set; }
        public DbSet<OrdOrderDetail> OrderDetails { get; set; }

        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrdOrderDetail>().HasOne<OrdOrder>().WithMany().HasForeignKey(o => o.OrderId).OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
