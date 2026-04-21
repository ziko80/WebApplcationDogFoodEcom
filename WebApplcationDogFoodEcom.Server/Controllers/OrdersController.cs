using Microsoft.AspNetCore.Mvc;
using WebApplcationDogFoodEcom.Server.Models;
using WebApplcationDogFoodEcom.Server.Services;

namespace WebApplcationDogFoodEcom.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _service;

    public OrdersController(IOrderService service) => _service = service;

    [HttpGet]
    public ActionResult<IReadOnlyList<Order>> GetAll() => Ok(_service.GetAll());

    [HttpGet("{id:int}")]
    public ActionResult<Order> GetById(int id)
    {
        var order = _service.GetById(id);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<Order>> Create(
        [FromBody] CreateOrderDto dto,
        CancellationToken ct)
    {
        var result = await _service.CreateAsync(dto, ct);
        if (!result.Success)
            return BadRequest(result.Error);

        var order = result.Value!;
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }
}
