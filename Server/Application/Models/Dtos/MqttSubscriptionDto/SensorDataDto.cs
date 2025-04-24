namespace Application.Models.Dtos.MqttSubscriptionDto;

public class SensorDataDto
{
    public float Temperature { get; set; } // Matches "temperature"
    public float Humidity { get; set; } // Matches "humidity"
    public float AirQuality { get; set; } // Matches "air_quality"
    public float Pm25 { get; set; } // Matches "pm25"
    public string DeviceId { get; set; } // Matches "device_id"
    public long Timestamp { get; set; } // Matches "timestamp"
}