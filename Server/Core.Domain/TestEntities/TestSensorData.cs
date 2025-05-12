namespace Core.Domain.TestEntities;

public partial class TestSensorData
{
    public int Id { get; set; }

    public double Temperature { get; set; }

    public double Humidity { get; set; }

    public double AirQuality { get; set; }

    public double Pm25 { get; set; }

    public Guid DeviceId { get; set; }

    public DateTime Timestamp { get; set; }
}
