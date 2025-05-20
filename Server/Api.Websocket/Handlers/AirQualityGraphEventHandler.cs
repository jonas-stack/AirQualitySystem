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
public class RequestAirQualityData : BaseDto
{
    //[JsonConverter(typeof(JsonStringEnumConverter))]
  //  public GraphType GraphType { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TimePeriod TimePeriod { get; set; }
    
    public required List<string> Data { get; set; }
}

// hvad server sender..
public class AirQualityDataGraph : BaseDto
{
    public List<string> RequestedData { get; set; }
    public TimePeriod TimePeriod { get; set; }
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
        
        var response = new WebsocketMessage<AirQualityDataGraph>
        {
            Topic = WebsocketTopics.Dashboard,
            Data = new AirQualityDataGraph
            {
                RequestedData = dto.Data,
                TimePeriod = dto.TimePeriod,
                Data = flattenedData
            }
        };
        
        await socket.Send(JsonSerializer.Serialize(response));
    }
}