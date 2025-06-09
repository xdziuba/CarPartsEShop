namespace CarPartsEShop.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Deleted { get; set; } = false;
}
