using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarPartsEShop.Repositories;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CarPartsEShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public OrdersController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: api/<OrdersController>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetOrders()
        {
            int customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var orders = await _context.Orders
                .Where(o => o.CustomerId == customerId)
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .ToListAsync();

            if (orders == null || !orders.Any()) 
            {
                return NotFound("No orders found!");
            }

            return Ok(orders);
        }
    }
}
