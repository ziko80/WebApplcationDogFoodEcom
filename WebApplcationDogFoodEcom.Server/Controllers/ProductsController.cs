using Microsoft.AspNetCore.Mvc;
using WebApplcationDogFoodEcom.Server.Models;
using WebApplcationDogFoodEcom.Server.Services;

namespace WebApplcationDogFoodEcom.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> Search(
        [FromQuery] ProductCategory? category,
        [FromQuery] string? search,
        CancellationToken ct)
        => Ok(await _service.SearchAsync(category, search, ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetById(int id, CancellationToken ct)
    {
        var product = await _service.GetByIdAsync(id, ct);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpGet("medicines")]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetMedicines(CancellationToken ct)
        => Ok(await _service.GetByCategoryAsync(ProductCategory.Medicine, ct));

    [HttpGet("vaccines")]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetVaccines(CancellationToken ct)
        => Ok(await _service.GetByCategoryAsync(ProductCategory.Vaccine, ct));
}
