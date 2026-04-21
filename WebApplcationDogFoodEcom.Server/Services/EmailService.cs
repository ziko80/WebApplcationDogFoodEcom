using MailKit.Net.Smtp;
using MimeKit;

namespace WebApplcationDogFoodEcom.Server.Services;

public class EmailSettings
{
    public string SmtpHost { get; set; } = "smtp.gmail.com";
    public int SmtpPort { get; set; } = 587;
    public string SenderEmail { get; set; } = "";
    public string SenderName { get; set; } = "PawMeds Store";
    public string SenderPassword { get; set; } = "";
}

public class EmailService
{
    private readonly EmailSettings _settings;

    public EmailService(EmailSettings settings)
    {
        _settings = settings;
    }

    public async Task SendInvoiceEmailAsync(string toEmail, string customerName, int orderId, byte[] pdfBytes)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        message.To.Add(new MailboxAddress(customerName, toEmail));
        message.Subject = $"PawMeds — Invoice for Order #{orderId:D5}";

        var builder = new BodyBuilder
        {
            HtmlBody = $"""
                <div style="font-family:Segoe UI,sans-serif;max-width:600px;margin:0 auto">
                    <h2 style="color:#4f46e5">🐾 PawMeds</h2>
                    <p>Hi <strong>{customerName}</strong>,</p>
                    <p>Thank you for your order! Please find your invoice attached.</p>
                    <table style="width:100%;border-collapse:collapse;margin:20px 0">
                        <tr style="background:#4f46e5;color:#fff">
                            <td style="padding:10px;font-weight:bold">Order #</td>
                            <td style="padding:10px">{orderId:D5}</td>
                        </tr>
                    </table>
                    <p>If you have any questions, reply to this email or contact us at <a href="mailto:support@pawmeds.com">support@pawmeds.com</a>.</p>
                    <p style="color:#64748b;font-size:12px;margin-top:30px">
                        &copy; 2025 PawMeds — Dog Medicine &amp; Vaccine Store
                    </p>
                </div>
                """
        };

        builder.Attachments.Add($"PawMeds_Invoice_{orderId:D5}.pdf", pdfBytes, new ContentType("application", "pdf"));
        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_settings.SenderEmail, _settings.SenderPassword);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
