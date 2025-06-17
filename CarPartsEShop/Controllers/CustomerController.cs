using CarPartsEShop.Dtos;
using CarPartsEShop.Models;
using CarPartsEShop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CarPartsEShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var result = await _customerService.GetCustomerAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Customer>> CreateCustomer(CreateCustomerDto dto)
        {
            var customer = new Customer
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                Street = dto.Street,
                City = dto.City,
                PostalCode = dto.PostalCode,
                Country = dto.Country,
                Password = dto.Password,
                Role = dto.Role
            };

            var result = await _customerService.CreateCustomerAsync(customer);
            return result;
        }
    }
}
