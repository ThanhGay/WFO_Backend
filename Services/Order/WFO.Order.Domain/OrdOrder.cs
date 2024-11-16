using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFO.Shared.Constant.Database;

namespace WFO.Order.Domain
{
    [Table(nameof(OrdOrder), Schema = DbSchema.Order)]
    public class OrdOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? CanceledDate { get; set; }

        /// <summary>
        /// 0 - created
        /// 1 - confirmed
        /// 2 - shipping
        /// 3 - received
        /// 5 - done (after payment)
        /// 10 - canceled
        /// </summary>
        [AllowedValues([0, 1, 2, 3, 5, 10], ErrorMessage = "This status is not define")]
        public int Status { get; set; } = 0;
    }
}
