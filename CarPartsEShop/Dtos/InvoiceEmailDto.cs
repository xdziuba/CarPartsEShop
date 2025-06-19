namespace CarPartsEShop.Dtos;

public class InvoiceEmailDto
{
    public required string ToEmail { get; set; } = default!;
    public required string CustomerName { get; set; } = default!;
    public required string InvoiceNumber { get; set; } = default!;
    public required DateTime Date { get; set; }
    public required decimal TotalAmount { get; set; }
    public required bool IsInvoice { get; set; } // true = Faktura, false = Paragon
}
