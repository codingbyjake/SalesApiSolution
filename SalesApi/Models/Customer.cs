using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesApi.Models {
    public class Customer {

        public int Id { get; set; }
        [StringLength(30)]
        public string Name { get; set; } = string.Empty;
        [StringLength(30)]
        public string City { get; set; } = string.Empty;
        [StringLength(2)]
        public string StateCode { get; set; } = string.Empty;
        [Column(TypeName = "decimal(9,2)")]
        public decimal Sales { get; set; } = 0;
        public bool Active { get; set; } = true;

        public Customer() { }

    }
}
