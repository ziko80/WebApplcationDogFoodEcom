using Microsoft.AspNetCore.Mvc;
using WebApplcationDogFoodEcom.Server.Services;

namespace WebApplcationDogFoodEcom.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BillingController : ControllerBase
{
    private readonly IBillingService _service;

    public BillingController(IBillingService service) => _service = service;

    [HttpGet("invoice/{orderId:int}")]
    public IActionResult DownloadInvoice(int orderId)
    {
        var result = _service.GetInvoice(orderId);
        if (!result.Success) return NotFound(result.Error);

        var invoice = result.Value!;
        return File(invoice.Pdf, "application/pdf", invoice.FileName);
    }

    [HttpPost("send-invoice/{orderId:int}")]
    public async Task<IActionResult> SendInvoice(int orderId, CancellationToken ct)
    {
        var result = await _service.EmailInvoiceAsync(orderId, ct);
        if (!result.Success)
            return result.Error!.StartsWith("Order not found") ? NotFound(result.Error) : Problem(result.Error);

        return Ok(new { message = result.Value });
    }
}
