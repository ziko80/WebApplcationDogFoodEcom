namespace WebApplcationDogFoodEcom.Server.Models;

public record ChatRequest(string Message, List<ChatTurn>? History = null);

public record ChatTurn(string Role, string Content);

public record ChatResponse(string Reply, List<ProductSuggestion>? Suggestions = null);

public record ProductSuggestion(int ProductId, string Name, string Reason);
