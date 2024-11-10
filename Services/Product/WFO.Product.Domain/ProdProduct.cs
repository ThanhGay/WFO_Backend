using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFO.Shared.Constant.Database;

namespace WFO.Product.Domain
{
    [Table(nameof(ProdProduct), Schema = DbSchema.Product)]
    public class ProdProduct
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(128)]
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int Price { get; set; }
        public string? Image { get; set; }
        public required string Size { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
