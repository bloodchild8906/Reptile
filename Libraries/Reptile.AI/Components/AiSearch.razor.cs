namespace Reptile.AI.Components;

public partial class AiSearch
{
    private string answer = string.Empty;
    private ChatRequest chatRequest;
    private string? errorMessage;
    private bool loading = false;
    private OpenAiApi openAiApi = new("sk-proj-Eem1WiY55v9JJF65EtnaT3BlbkFJ2PVEWkPYFfzIMfykxFyL");
    private List<string>? searchResults;
    [Inject] private IJSRuntime JsRuntime { get; set; }
    private string SearchQuery { get; set; }

    private async Task PerformSearch()
    {
        chatRequest ??= new ChatRequest();

        if (chatRequest.Messages is null)
            chatRequest.Messages = new List<ChatMessage>();

        chatRequest.Messages?.Add(new ChatMessage { Content = SearchQuery, Role = ChatMessageRole.User });
        loading = true;
        errorMessage = null;
        searchResults = [];

        try
        {
            await openAiApi.Chat.StreamCompletionAsync(chatRequest, (index, result) =>
            {
                // Check if the result has any completions and handle them
                if (result.Choices != null && result.Choices.Any())
                {
                    searchResults.AddRange(result.Choices.Select(c => c.Delta?.Content ?? ""));
                    if (searchResults is not null)
                        answer = string.Join("", searchResults);
                    InvokeAsync(StateHasChanged); // Update UI with new results
                }
            });
        }
        catch (Exception ex)
        {
            errorMessage = $"Failed to perform search: {ex.Message}";
            InvokeAsync(StateHasChanged); // Update UI on error
        }

        loading = false;
        InvokeAsync(StateHasChanged); // Final update to turn off loading indicator and show results or error
    }
}