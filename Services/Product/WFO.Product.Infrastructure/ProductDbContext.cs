using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFO.Product.Domain;

namespace WFO.Product.Infrastructure
{
    public class ProductDbContext : DbContext
    {
        public DbSet<ProdProduct> Products { get; set; }
        public DbSet<ProdCategory> Categories { get; set; }
        public DbSet<ProdProductCategory> ProductCategories { get; set; }

        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProdProductCategory>().HasOne<ProdProduct>().WithMany().HasForeignKey(p => p.ProductId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProdProductCategory>().HasOne<ProdCategory>().WithMany().HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
