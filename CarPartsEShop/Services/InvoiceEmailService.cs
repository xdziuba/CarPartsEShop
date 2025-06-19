using System.Net;
using System.Net.Mail;
using System.Text;
using CarPartsEShop.Dtos;
using CarPartsEShop.Services;

public class InvoiceEmailService : IInvoiceEmailService
{
    private readonly IConfiguration _config;

    public InvoiceEmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendInvoiceAsync(InvoiceEmailDto dto)
    {
        var from = _config["EmailSettings:From"];
        var smtpHost = _config["EmailSettings:SmtpHost"];
        var smtpPort = int.Parse(_config["EmailSettings:SmtpPort"]);
        var smtpUser = _config["EmailSettings:SmtpUser"];
        var smtpPass = _config["EmailSettings:SmtpPass"];

        var mail = new MailMessage
        {
            From = new MailAddress(from),
            Subject = dto.IsInvoice ? $"Faktura #{dto.InvoiceNumber}" : $"Paragon #{dto.InvoiceNumber}",
            Body = GenerateEmailBody(dto),
            IsBodyHtml = false
        };

        mail.To.Add(dto.ToEmail);

        using var smtp = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUser, smtpPass),
            EnableSsl = true
        };

        await smtp.SendMailAsync(mail);
    }

    private string GenerateEmailBody(InvoiceEmailDto dto)
    {
        var type = dto.IsInvoice ? "Faktura VAT" : "Paragon";
        var sb = new StringBuilder();
        sb.AppendLine($"Witaj {dto.CustomerName},");
        sb.AppendLine($"Dziękujemy za zakupy. W załączeniu przesyłamy {type}.");
        sb.AppendLine();
        sb.AppendLine($"Numer: {dto.InvoiceNumber}");
        sb.AppendLine($"Data: {dto.Date.ToShortDateString()}");
        sb.AppendLine($"Kwota: {dto.TotalAmount} zł");
        sb.AppendLine();
        sb.AppendLine("Z poważaniem,");
        sb.AppendLine("Zespół CarPartsEShop");

        return sb.ToString();
    }
}