using Application.Interfaces.Infrastructure.Ai;

namespace Infrastructure.Ai;

public class AiApiService : IAiService
{
    private readonly HttpClient _httpClient;
    
    public AiApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> Ai_chat(string message)
    {
        var fullUrl = new Uri(_httpClient.BaseAddress, $"/AetherAI/ask_ai?question={message}");
        Console.WriteLine(fullUrl.ToString());
        var response = await _httpClient.GetAsync($"/AetherAI/ask_ai?question={message}"); ;
        return await response.Content.ReadAsStringAsync();
    }
}
