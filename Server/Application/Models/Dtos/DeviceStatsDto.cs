namespace Application.Models.Dtos;

public class DeviceStatsDto
{
    public required int AllTimeMeasurements { get; set; }
    public required int ConnectedDevices { get; set; }
    public required int DisconnectionsLast24Hours { get; set; }
}