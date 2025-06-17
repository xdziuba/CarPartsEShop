using CarPartsEShop.Models;
using CarPartsEShop.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CarPartsEShop.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;
        public CartService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<CartItem> AddItemAsync(int cartId, int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new ArgumentException("Product does not exist.");

            var item = new CartItem { CartId = cartId, ProductId = productId, Quantity = quantity };
            _context.CartItems.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<CheckoutResponse> CheckoutAsync(int customerId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null || !cart.Items.Any())
            {
                return new CheckoutResponse
                {
                    Success = false,
                    Msg = "Cart is empty or does not exist."
                };
            }

            foreach (var item in cart.Items)
            {
                if (item.Product.Stock < item.Quantity)
                {
                    return new CheckoutResponse
                    {
                        Success = false,
                        Msg = $"Insufficient stock for product {item.Product.Name}."
                    };
                }
            }

            foreach (var item in cart.Items)
            {
                item.Product.Stock -= item.Quantity;
            }

            var order = new Order
            {
                CustomerId = customerId,
                Items = cart.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Product.Price
                }).ToList()
            };

            await _context.Orders.AddAsync(order);

            cart.Items.Clear();
            await _context.SaveChangesAsync();

            return new CheckoutResponse
            {
                Success = true,
                Msg = "Checkout successful."
            };
        }

        public async Task<Cart> CreateCartAsync(int customerId)
        {
            var customerExists = await _context.Customers.AnyAsync(c => c.Id == customerId);
            if (!customerExists)
                throw new ArgumentException("Customer does not exist.");
            
            var cart = new Cart { CustomerId = customerId };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<List<Cart>> GetAllCartsAsync()
        {
            return await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .ToListAsync();
        }

        public async Task<Cart?> GetCartAsync(int customerId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        public async Task<CartItem?> GetCartItemByIdAsync(int itemId)
        {
            return await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Id == itemId);
        }

        public async Task<bool> RemoveItemAsync(int itemId)
        {
            var item = await _context.CartItems.FindAsync(itemId);
            if (item == null) return false;
            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CartItem?> UpdateItemAsync(int itemId, int quantity)
        {
            var item = await _context.CartItems.FindAsync(itemId);
            if (item == null) return null;
            item.Quantity = quantity;
            await _context.SaveChangesAsync();
            return item;
        }
    }
}
