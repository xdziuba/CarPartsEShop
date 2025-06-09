namespace CarPartsEShop.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Ean { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; } = 0;
    public int CategoryId { get; set; } //FK
    public Category Category { get; set; } = default!;
    public bool Deleted { get; set; } = false;
}
