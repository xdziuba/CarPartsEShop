using CarPartsEShop.Models;

namespace CarPartsEShop.Services
{
    public interface ICartService
    {
        Task<Cart?> GetCartAsync(int customerId);
        Task<Cart> CreateCartAsync(int customerId);
        Task<CartItem> AddItemAsync(int cartId, int productId, int quantity);
        Task<CartItem?> UpdateItemAsync(int itemId, int quantity);
        Task<bool> RemoveItemAsync(int itemId);
    }
}
