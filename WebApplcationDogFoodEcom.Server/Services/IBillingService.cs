namespace WebApplcationDogFoodEcom.Server.Services;

public record InvoiceDownload(byte[] Pdf, string FileName);

public interface IBillingService
{
    ServiceResult<InvoiceDownload> GetInvoice(int orderId);
    Task<ServiceResult<string>> EmailInvoiceAsync(int orderId, CancellationToken ct = default);
}
