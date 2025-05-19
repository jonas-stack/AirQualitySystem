using Application.Interfaces;
using Application.Interfaces.Infrastructure.Ai;
using Application.Models.Dtos.Ai;

namespace Application.Services;

public class AiCommunicationService(IAiService aiService) : IAiCommunication
{
    public async Task<string> Ai_chat(string message)
    {
        return await aiService.Ai_chat(message);
    }

    public async Task<string> AnalyzeHistoricalData(string range)
    {
        return await aiService.AnalyzeHistoricalData(range);
    }

    public async Task<string> AnalyzeLiveData()
    {
        return await aiService.AnalyzeLiveData();
    }
}