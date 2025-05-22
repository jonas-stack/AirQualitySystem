using System.Text.Json;
using Application.Interfaces;
using Application.Models;
using Application.Models.Dtos.Ai;
using Application.Models.Websocket;
using Fleck;
using WebSocketBoilerplate;

namespace Api.Websocket.Handlers;

public class ClientRequestAiLiveData : BaseDto {} 

public class AiLiveDataEventHandler : BaseEventHandler<ClientRequestAiLiveData>
{
    
    private readonly IAiCommunication _aiCommunication;
    
    public AiLiveDataEventHandler(IAiCommunication aiCommunication)
    {
        _aiCommunication = aiCommunication;
    }
    
    public override async Task Handle(ClientRequestAiLiveData dto, IWebSocketConnection socket)
    {
        var result = await _aiCommunication.AnalyzeLiveData();

        var response = new WebsocketMessage<LiveAiFeedbackDto>
        {
            Topic = WebsocketTopics.Ai,
            Data = new LiveAiFeedbackDto
            {
                AiAdvice = result
            }
        };
        
        await socket.Send(JsonSerializer.Serialize(response));
    }
}