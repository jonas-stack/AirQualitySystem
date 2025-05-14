using System.Text.Json.Serialization;

namespace Application.Models.Dtos.MQTT;

public class DeviceConnectionDto
{
    [JsonPropertyName("device_id")]
    public Guid DeviceGuid { get; set; }
    
    [JsonPropertyName("DeviceName")]
    public string DeviceName { get; set; }
    
    [JsonPropertyName("LastSeen")]
    public DateTime LastSeen { get; set; }
    
    [JsonPropertyName("IsConnected")]
    public bool IsConnected { get; set; }
}