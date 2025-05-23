namespace Application.Models.Websocket;

public static class WebsocketTopics
{
    public const string Dashboard = nameof(Dashboard);
    public const string Ai = nameof(Ai);
    public const string DeviceStatus = nameof (DeviceStatus);
    public const string DeviceData = nameof (DeviceData);
    public const string Device = nameof (Device);

    /* ---- Graphs ---- */
    public const string Graph = nameof (Graph);
    public const string GraphTotalMeasurements = nameof(GraphTotalMeasurements);
}