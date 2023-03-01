namespace SalesApi.Models {
    public class OrderLine {
        //PK and Class properties
        public int Id { get; set; }
        public int Quantity { get; set; } = 1;

        //FK's and virtual properties
        public int OrderId { get; set; }
        public virtual Order Order { get; set; } = null!;

        public int ItemId { get; set; }
        public virtual Item Item { get; set; } = null!;
    }
}
