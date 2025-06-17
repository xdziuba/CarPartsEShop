using CarPartsEShop.Dtos;
using CarPartsEShop.Models;
using CarPartsEShop.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CarPartsEShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthenticationController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid registration data.");
            }
            if (string.IsNullOrWhiteSpace(dto.FullName) || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest("Full name, email, and password are required.");
            }

            var exists = await _context.Customers.AnyAsync(c => c.Email == dto.Email);
            if (exists)
            {
                return BadRequest("Email already exists.");
            }

            var customer = new Customer
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Password = dto.Password // In a real application, you should hash the password before saving it
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return Ok("Successfully registered!");
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDto dto)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == dto.Email);
            if (customer == null || customer.Password != dto.Password) // In a real application, you should hash the password and compare hashes
            {
                return Unauthorized("Invalid email or password.");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
                new Claim(ClaimTypes.Email, customer.Email),
                new Claim(ClaimTypes.Role, customer.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Test key, thanks to which we will pass the project with at least a 4 :)"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "car-parts.shop",
                audience: "car-parts.shop",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(tokenString);
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword(string email, string oldPassword, string newPassword)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            if (customer.Password != oldPassword) // In a real application, you should hash the password and compare hashes
            {
                return Unauthorized("Old password is incorrect.");
            }

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                return BadRequest("New password cannot be empty.");
            }

            customer.Password = newPassword; // In a real application, you should hash the password before saving it
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
            return Ok("Password reset successfully.");
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<ActionResult> UpdateCustomer(UpdateCustomerDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid update data.");
            }

            var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            customer.FullName = dto.FullName;
            customer.Phone = dto.Phone;
            customer.Street = dto.Street;
            customer.City = dto.City;
            customer.PostalCode = dto.PostalCode;
            customer.Country = dto.Country;

            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
            return Ok("Customer updated successfully.");
        }
    }
}
