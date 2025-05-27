using System.Text.Json.Serialization;

namespace Application.Models.Dtos;

//pascal for json😎
public class DeviceConnectionHistoryDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("deviceId")]
    public required string DeviceId { get; set; }

    [JsonPropertyName("isConnected")]
    public bool IsConnected { get; set; }

    [JsonPropertyName("lastSeen")]
    public long LastSeen { get; set; }
    
    // vi kan finde ud af hvor længe nedetiden var
    [JsonPropertyName("duration")]
    public long? Duration { get; set; }
}