using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Enums;
using Application.Interfaces;
using Application.Models.Websocket;
using Application.Models.Websocket.Graph;
using Core.Domain.Entities;
using Fleck;
using WebSocketBoilerplate;

namespace Api.Websocket.Handlers;

// hvad klient sender
/*
 * {
      "eventType": "RequestAirQualityData",
      "timePeriod": "Weekly",
      "data": ["temperature", "humidity", "pm25"]
 * }
 */
public class RequestAirQualityData : BaseDto
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TimePeriod TimePeriod { get; set; }
    
    public required List<string> Data { get; set; }
}

// hvad server sender..
/*
 * {
    "Topic": "Dashboard",
    "Data": {
        "RequestedData": [
            "temperature",
            "humidity",
            "pm25"
        ],
        "TimePeriod": Weekly,
        "Data": [
            {
                "time": "Week 18",
                "temperature": 23.2,
                "humidity": 35.39,
                "pm25": 6.8
            },
        ],
        "eventType": "AirQualityDataGraph",
        "requestId": null
    }
}
 */
public class ServerResponseAirQualityData : BaseDto
{
    [JsonPropertyName("requestedData")]
    public required List<string> RequestedData { get; set; }
    
    [JsonPropertyName("timePeriod")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TimePeriod TimePeriod { get; set; }
    
    [JsonPropertyName("data")]
    public required List<Dictionary<string, object>> Data { get; set; }
}

public class AirQualityGraphEventHandler(IGraphService graphService) : BaseEventHandler<RequestAirQualityData>
{
    public override async Task Handle(RequestAirQualityData dto, IWebSocketConnection socket)
    {
        var result = await graphService.GetFlexibleGraphDataAsync(dto.Data, dto.TimePeriod);

        var flattenedData = result.Select(item =>
        {
            var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                ["time"] = item.Time
            };

            foreach (var kvp in item.DataPoints)
            {
                dict[kvp.Key] = kvp.Value;
            }

            return dict;
        }).ToList();
        
        var response = new ServerResponseAirQualityData
        {
            requestId = dto.requestId,
            eventType = nameof(ServerResponseAirQualityData),
            RequestedData = dto.Data,
            TimePeriod = dto.TimePeriod,
            Data = flattenedData
        };
        
        await socket.Send(JsonSerializer.Serialize(response));
    }
}