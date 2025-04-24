namespace Core.Domain.Entities;

public class SensorData
{
    public int Id { get; set; } // Primary key
    public float Temperature { get; set; }
    public float Humidity { get; set; }
    public float AirQuality { get; set; }
    public float Pm25 { get; set; }
    public string DeviceId { get; set; }
    public long Timestamp { get; set; }
}