using Application.Interfaces;
using Application.Interfaces.Infrastructure.Ai;
using Application.Interfaces.Infrastructure.Websocket;
using Application.Models;
using Application.Models.Dtos.Ai;
using Application.Models.Websocket;

namespace Application.Services;

public class AiCommunicationService(IAiService aiService, IConnectionManager connectionManager) : IAiCommunication
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
    
    public async Task BroadCastData()
    {
        var result = await AnalyzeLiveData();
        
        Console.WriteLine($"[Broadcasting] Topic: {WebsocketTopics.Ai}");
        Console.WriteLine($"[Broadcasting] AI Advice: {result.Substring(0, Math.Min(100, result.Length))}...");
        
        var response = new WebsocketMessage<LiveAiFeedbackDto>
            {
                Topic = WebsocketTopics.Ai,
                Data = new LiveAiFeedbackDto()
                {
                    AiAdvice = result
                } 
            };
                
        await connectionManager.BroadcastToTopic(WebsocketTopics.Ai, response);
    }
}