namespace CarPartsEShop.Models;

public class CartItem
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Qty { get; set; }
    public Product Product { get; set; } = default!;
}
