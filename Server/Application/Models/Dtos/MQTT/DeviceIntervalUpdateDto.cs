using System.Text.Json.Serialization;
using WebSocketBoilerplate;

namespace Application.Models.Dtos.MQTT;

public class DeviceIntervalUpdateDto : BaseDto
{
    [JsonPropertyName("deviceId")]
    public required string DeviceId { get; set; }
    
    [JsonPropertyName("interval")]
    public int Interval { get; set; }
}
