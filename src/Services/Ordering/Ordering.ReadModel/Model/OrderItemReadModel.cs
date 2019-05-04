using System;

namespace Ordering.ReadModel.Model
{
    public class OrderItemReadModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Units { get; set; }
        public decimal UnitPrice { get; set; }
        public string PictureUrl { get; set; }
    }
}
