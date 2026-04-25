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

    [HttpGet("accessories")]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetAccessories(CancellationToken ct)
    {
        var all = await _service.SearchAsync(null, null, ct);
        var results = all.Where(p => p.Category is not (ProductCategory.Medicine or ProductCategory.Vaccine)).ToList();
        return Ok(results);
    }

    [HttpGet("toys")]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetToys(CancellationToken ct)
        => Ok(await _service.GetByCategoryAsync(ProductCategory.Toys, ct));

    [HttpGet("feeding")]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetFeeding(CancellationToken ct)
        => Ok(await _service.GetByCategoryAsync(ProductCategory.Feeding, ct));

    [HttpGet("grooming")]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetGrooming(CancellationToken ct)
        => Ok(await _service.GetByCategoryAsync(ProductCategory.Grooming, ct));

    [HttpGet("hygiene")]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetHygiene(CancellationToken ct)
        => Ok(await _service.GetByCategoryAsync(ProductCategory.Hygiene, ct));

    [HttpGet("travel")]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetTravel(CancellationToken ct)
        => Ok(await _service.GetByCategoryAsync(ProductCategory.Travel, ct));
}
