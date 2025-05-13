using System.Text.Json.Serialization;

namespace Application.Models.Dtos.MQTT;

public class ConnectionEventDto
{
    public Guid DeviceGuid { get; set; }
    public string DeviceName { get; set; } 
    public DateTime Timestamp { get; set; }
    public bool IsConnected { get; set; }
}