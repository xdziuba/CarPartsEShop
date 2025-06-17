﻿namespace CarPartsEShop.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public List<OrderItem> Items { get; set; } = new();
    }
}
