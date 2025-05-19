using System.Text.Json.Serialization;
using Application.Models.Websocket;

namespace Application.Models.Dtos.MQTT;

public class DeviceDto
{
    [JsonPropertyName("device_id")]
    public string DeviceGuid { get; set; }
    
    [JsonPropertyName("DeviceName")]
    public string DeviceName { get; set; }
    
    [JsonPropertyName("LastSeen")]
    public long LastSeen { get; set; }
    
    [JsonPropertyName("IsConnected")]
    public bool IsConnected { get; set; }
}