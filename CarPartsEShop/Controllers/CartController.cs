using CarPartsEShop.Models;
using CarPartsEShop.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CarPartsEShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }


        [HttpGet("{customerId}")]
        public async Task<ActionResult<Cart>> GetCart(int customerId)
        {
            var cart = await _cartService.GetCartAsync(customerId);
            if (cart == null)
                return NotFound();
            return Ok(cart);
        }

        [HttpPost("create/{customerId}")]
        public async Task<ActionResult<Cart>> CreateCart(int customerId)
        {
            var cart = await _cartService.CreateCartAsync(customerId);
            return Ok(cart);
        }

        [HttpPost("add-item")]
        public async Task<ActionResult<CartItem>> AddItem(int cartId, int productId, int quantity)
        {
            var item = await _cartService.AddItemAsync(cartId, productId, quantity);
            return Ok(item);
        }

        [HttpPut("update-item/{itemId}")]
        public async Task<ActionResult<CartItem>> UpdateItem(int itemId, int quantity)
        {
            var updated = await _cartService.UpdateItemAsync(itemId, quantity);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("remove-item/{itemId}")]
        public async Task<ActionResult> RemoveItem(int itemId)
        {
            var removed = await _cartService.RemoveItemAsync(itemId);
            if (!removed) return NotFound();
            return Ok();
        }
    }
}
