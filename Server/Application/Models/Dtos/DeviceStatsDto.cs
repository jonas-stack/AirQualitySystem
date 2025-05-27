using System.Text.Json.Serialization;
using WebSocketBoilerplate;

namespace Application.Models.Dtos;

public class DeviceStatsDto
{
    [JsonPropertyName("allTimeMeasurements")]
    public required int AllTimeMeasurements { get; set; }
    
    [JsonPropertyName("connectedDevices")]
    public required int ConnectedDevices { get; set; }
    
    [JsonPropertyName("disconnectionsLast24Hours")]
    public required int DisconnectionsLast24Hours { get; set; }
}