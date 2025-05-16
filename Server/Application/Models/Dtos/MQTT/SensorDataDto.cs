using System.Text.Json.Serialization;

namespace Application.Models.Dtos.MQTT;

public class SensorDataDto
{
    [JsonPropertyName("temperature")]
    public double Temperature { get; set; }
    
    [JsonPropertyName("humidity")]
    public double Humidity { get; set; }
    
    [JsonPropertyName("air_quality")]
    public double AirQuality { get; set; }
    
    [JsonPropertyName("pm25")]
    public double Pm25 { get; set; }
    
    [JsonPropertyName("device_id")]
    public string DeviceId { get; set; } = null!;// recieves a string id in the json message from broker. parse it to guid before saving in db
    
    [JsonPropertyName("timestamp")]
    public long TimestampUnix { get; set; }
    
}