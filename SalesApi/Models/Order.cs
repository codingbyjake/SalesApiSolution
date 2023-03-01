using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesApi.Models {
    public class Order {

        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        [StringLength(30)]
        public string Description { get; set; } = string.Empty;
        [Column(TypeName = "decimal(7,2)")]
        public decimal Total { get; set; } = 0;

        public int? CustomerId { get; set; } = null;
        public virtual Customer? Customer { get; set; }

        public virtual ICollection<OrderLine>? OrderLines { get; set; }
    }
}
