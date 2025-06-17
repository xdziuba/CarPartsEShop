using CarPartsEShop.Models;
using CarPartsEShop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        // --- Admin-only endpoints ---

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<Cart>>> GetAllCarts()
        {
            var carts = await _cartService.GetAllCartsAsync();
            return Ok(carts);
        }

        [HttpGet("{customerId}")]
        [Authorize (Roles = "Admin")]
        public async Task<ActionResult<Cart>> GetCart(int customerId)
        {
            var cart = await _cartService.GetCartAsync(customerId);
            if (cart == null)
                return NotFound();
            return Ok(cart);
        }

        // --- Customer endpoints ---

        [HttpPost("checkout")]
        [Authorize]
        public async Task<ActionResult<CheckoutResponse>> Checkout()
        {
            int customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var response = await _cartService.CheckoutAsync(customerId);
            if (!response.Success)
                return BadRequest(new { message = response.Msg });
            return Ok(response);
        }

        [HttpGet("my-cart")]
        [Authorize]
        public async Task<ActionResult<Cart>> GetMyCart()
        {
            int customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var cart = await _cartService.GetCartAsync(customerId);
            if (cart == null)
                return NotFound();
            return Ok(cart);
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<ActionResult<Cart>> CreateCart()
        {
            int customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            try
            {
                var cart = await _cartService.CreateCartAsync(customerId);
                return Ok(cart);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("add-item")]
        [Authorize]
        public async Task<ActionResult<CartItem>> AddItem(int cartId, int productId, int quantity)
        {
            int customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var cart = await _cartService.GetCartAsync(cartId);
            if (cart == null || (cart.CustomerId != customerId && !User.IsInRole("Admin"))) return Forbid("You don't have permisssions to edit this cart!");

            try
            {
                var item = await _cartService.AddItemAsync(cartId, productId, quantity);
                return Ok(item);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("update-item/{itemId}")]
        [Authorize]
        public async Task<ActionResult<CartItem>> UpdateItem(int itemId, int quantity)
        {
            int customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var item = await _cartService.GetCartItemByIdAsync(itemId);
            if (item == null || (item.Cart.CustomerId != customerId && !User.IsInRole("Admin")))
                return Forbid("You don't have permissions to edit this item!");

            var updated = await _cartService.UpdateItemAsync(itemId, quantity);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("remove-item/{itemId}")]
        [Authorize]
        public async Task<ActionResult> RemoveItem(int itemId)
        {
            int customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var item = await _cartService.GetCartItemByIdAsync(itemId);
            if (item == null || (item.Cart.CustomerId != customerId && !User.IsInRole("Admin")))
                return Forbid("You don't have permissions to remove this item!");

            var removed = await _cartService.RemoveItemAsync(itemId);
            if (!removed) return NotFound();
            return Ok();
        }
    }
}
