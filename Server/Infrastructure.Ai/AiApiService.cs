using Application.Interfaces.Infrastructure.Ai;

namespace Infrastructure.Ai;

public class AiApiService(HttpClient httpClient) : IAiService
{
    public async Task<string> Ai_chat(string message)
    {
        var endpoint = new Uri(httpClient.BaseAddress, $"/AetherAI/ask_ai?question={message}");
        Console.WriteLine(endpoint.ToString());
        var response = await httpClient.GetAsync($"/AetherAI/ask_ai?question={message}");
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> AnalyzeHistoricalData(string range)
    {
        var endpoint = $"/AetherAI/historical_analysis?range={range}";
        var response = await httpClient.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> AnalyzeLiveData()
    {
        var response = await httpClient.GetAsync("/AetherAI/analyze_live_data");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
