namespace WebApplcationDogFoodEcom.Server.Services;

public class BillingService : IBillingService
{
    private readonly IOrderService _orders;
    private readonly EmailService _email;

    public BillingService(IOrderService orders, EmailService email)
    {
        _orders = orders;
        _email = email;
    }

    public ServiceResult<InvoiceDownload> GetInvoice(int orderId)
    {
        var order = _orders.GetById(orderId);
        if (order is null)
            return ServiceResult<InvoiceDownload>.Fail("Order not found");

        var pdf = InvoicePdfService.Generate(order);
        return ServiceResult<InvoiceDownload>.Ok(
            new InvoiceDownload(pdf, $"PawMeds_Invoice_{orderId:D5}.pdf"));
    }

    public async Task<ServiceResult<string>> EmailInvoiceAsync(int orderId, CancellationToken ct = default)
    {
        var order = _orders.GetById(orderId);
        if (order is null)
            return ServiceResult<string>.Fail("Order not found");

        var pdf = InvoicePdfService.Generate(order);

        try
        {
            await _email.SendInvoiceEmailAsync(order.CustomerEmail, order.CustomerName, order.Id, pdf);
            return ServiceResult<string>.Ok($"Invoice emailed to {order.CustomerEmail}");
        }
        catch (Exception ex)
        {
            return ServiceResult<string>.Fail($"Failed to send email: {ex.Message}");
        }
    }
}
