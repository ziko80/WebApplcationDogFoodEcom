using Microsoft.AspNetCore.Mvc;
using WebApplcationDogFoodEcom.Server.Models;
using WebApplcationDogFoodEcom.Server.Services;

namespace WebApplcationDogFoodEcom.Server.Controllers;

[ApiController]
[Route("api/ai")]
public class AiController : ControllerBase
{
    private readonly AiAssistantService _assistant;

    public AiController(AiAssistantService assistant) => _assistant = assistant;

    [HttpPost("chat")]
    public async Task<ActionResult<ChatResponse>> Chat(
        [FromBody] ChatRequest request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
            return BadRequest("Message is required.");

        var reply = await _assistant.ChatAsync(request, ct);
        return Ok(new ChatResponse(reply));
    }
}
