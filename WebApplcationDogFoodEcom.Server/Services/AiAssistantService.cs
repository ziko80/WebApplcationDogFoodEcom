using System.ComponentModel;
using Microsoft.Extensions.AI;
using WebApplcationDogFoodEcom.Server.Data;
using WebApplcationDogFoodEcom.Server.Models;

namespace WebApplcationDogFoodEcom.Server.Services;

/// <summary>
/// AI-powered pet-health assistant. Wraps <see cref="IChatClient"/> and exposes
/// product-catalog tools via function calling so the LLM can ground answers
/// in the real PawMeds catalog instead of hallucinating products.
/// </summary>
public class AiAssistantService
{
    private const string SystemPrompt = """
        You are "PawMeds Assistant", a friendly and cautious AI helper for a dog
        medicine & vaccine e-commerce store. Your job:
          - Answer pet-health questions in simple language.
          - Recommend products from the PawMeds catalog using the provided tools.
          - ALWAYS include a short disclaimer that the user should consult a
            licensed veterinarian before starting or changing any medication.
          - Never invent products that are not returned by the tools.
          - Keep replies concise (under 180 words) and structured with bullet points
            when listing products.
        """;

    private readonly IChatClient _chatClient;
    private readonly ILogger<AiAssistantService> _logger;
    private readonly ChatOptions _chatOptions;

    public AiAssistantService(IChatClient chatClient, ILogger<AiAssistantService> logger)
    {
        _chatClient = chatClient;
        _logger = logger;

        _chatOptions = new ChatOptions
        {
            Temperature = 0.3f,
            Tools =
            [
                AIFunctionFactory.Create(SearchProducts),
                AIFunctionFactory.Create(GetProductById),
                AIFunctionFactory.Create(ListByCategory),
            ]
        };
    }

    public async Task<string> ChatAsync(ChatRequest request, CancellationToken ct = default)
    {
        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, SystemPrompt)
        };

        if (request.History is { Count: > 0 })
        {
            foreach (var turn in request.History)
            {
                var role = turn.Role.Equals("assistant", StringComparison.OrdinalIgnoreCase)
                    ? ChatRole.Assistant
                    : ChatRole.User;
                messages.Add(new ChatMessage(role, turn.Content));
            }
        }

        messages.Add(new ChatMessage(ChatRole.User, request.Message));

        _logger.LogInformation("Sending chat request to LLM ({MessageCount} messages)", messages.Count);

        var response = await _chatClient.GetResponseAsync(messages, _chatOptions, ct);
        return response.Text ?? string.Empty;
    }

    // ---------- Tool functions (exposed to the LLM) ----------

    [Description("Searches the PawMeds product catalog by free-text query matching name, description, or target condition.")]
    private static IEnumerable<Product> SearchProducts(
        [Description("Free-text query, e.g. 'fleas', 'itchy skin', 'heartworm'.")] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return ProductStore.Products;

        return ProductStore.Products.Where(p =>
            p.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            p.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            p.TargetCondition.Contains(query, StringComparison.OrdinalIgnoreCase));
    }

    [Description("Gets a single product by its numeric id.")]
    private static Product? GetProductById(
        [Description("The product id.")] int id)
        => ProductStore.Products.FirstOrDefault(p => p.Id == id);

    [Description("Lists products filtered by category: 'Medicine' or 'Vaccine'.")]
    private static IEnumerable<Product> ListByCategory(
        [Description("Either 'Medicine' or 'Vaccine'.")] string category)
    {
        if (!Enum.TryParse<ProductCategory>(category, ignoreCase: true, out var cat))
            return [];
        return ProductStore.Products.Where(p => p.Category == cat);
    }
}
