using System.Text.Json.Serialization;

namespace Application.Models.Dtos.MQTT;

public class DeviceIntervalUpdateDto
{
    [JsonPropertyName("deviceId")]
    public required string DeviceId { get; set; }
    
    [JsonPropertyName("interval")]
    public int Interval { get; set; }
}
