using CarPartsEShop.Dtos;
using CarPartsEShop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarPartsEShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceEmailController : ControllerBase
    {
        private readonly IInvoiceEmailService _emailService;

        public InvoiceEmailController(IInvoiceEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SendInvoice([FromBody] InvoiceEmailDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ToEmail))
                return BadRequest("Invalid invoice data.");

            try
            {
                await _emailService.SendInvoiceAsync(dto);
                return Ok("Invoice sent successfully.");
            }
            catch (Exception ex)
            {
                // log if needed
                return StatusCode(500, $"Failed to send email: {ex.Message}");
            }
        }
    }
}