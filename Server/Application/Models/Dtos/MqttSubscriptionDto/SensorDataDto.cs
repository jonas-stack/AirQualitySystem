namespace Application.Models.Dtos.MqttSubscriptionDto;

public class SensorDataDto
{
    public string SensorId { get; set; }
    public double Value { get; set; }
    public DateTime Timestamp { get; set; }
}