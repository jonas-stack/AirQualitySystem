using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Interfaces;
using Application.Models;
using Application.Models.Dtos;
using Application.Models.Dtos.Ai;
using Application.Models.Dtos.MQTT;
using Application.Models.Websocket;
using Fleck;
using WebSocketBoilerplate;

namespace Api.Websocket.Handlers;

// klient skal sende noget, før vi kan sende data tilbage
// derfor requester klienten dette:
// { "eventType": "ClientRequestDeviceList" }
public class ClientRequestSensorData : BaseDto
{
    public required string SensorId { get; set; }
    
    // sæt default values
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

// serveren sender dette tilbage til klienten
public class ServerResponseSensorData : BaseDto
{
    [JsonPropertyName("sensorData")]
    public required PagedResult<SensorDataDto> SensorData { get; set; }
}

public class DeviceSensorDataHandler(ISensorDataService sensorDataService) : BaseEventHandler<ClientRequestSensorData>
{
    
    public override async Task Handle(ClientRequestSensorData dto, IWebSocketConnection socket)
    {
        var sensorData = await sensorDataService.GetSensorDataForDeviceAsync(dto.SensorId, dto.PageNumber, dto.PageSize);
        
        var response = new ServerResponseSensorData
        {
            eventType = "ServerResponseSensorData",
            requestId = dto.requestId,
            SensorData = sensorData,
        };
        
        await socket.Send(JsonSerializer.Serialize(response));
    }
}