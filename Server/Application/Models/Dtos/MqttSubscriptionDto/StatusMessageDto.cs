namespace Application.Models.Dtos.MqttSubscriptionDto;

public class StatusMessageDto
{
    public string Status { get; set; } // Matches "status"
    public string DeviceId { get; set; } // Matches "device_id"
    public long Timestamp { get; set; } // Matches "timestamp"
}