using Microsoft.AspNetCore.Mvc;
using WebApplcationDogFoodEcom.Server.Services;

namespace WebApplcationDogFoodEcom.Server.Controllers;

public record AddToCartRequest(int ProductId, int Quantity = 1);

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _service;

    public CartController(ICartService service) => _service = service;

    [HttpGet]
    public ActionResult<CartSnapshot> Get() => Ok(_service.Get());

    [HttpPost]
    public async Task<ActionResult<CartSnapshot>> Add([FromBody] AddToCartRequest request, CancellationToken ct)
    {
        var result = await _service.AddAsync(request.ProductId, request.Quantity, ct);
        return result.Success ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{productId:int}")]
    public ActionResult<CartSnapshot> Remove(int productId) => Ok(_service.Remove(productId));

    [HttpDelete]
    public ActionResult<CartSnapshot> Clear() => Ok(_service.Clear());
}
