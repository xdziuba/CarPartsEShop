using CarPartsEShop.Dtos;

namespace CarPartsEShop.Services;

public interface IInvoiceEmailService
{
    Task SendInvoiceAsync(InvoiceEmailDto dto);
}