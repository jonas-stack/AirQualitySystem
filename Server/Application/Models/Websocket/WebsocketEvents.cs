namespace Application.Models.Websocket;

public class WebsocketEvents
{
    public const string GraphTotalMeasurement = nameof(GraphTotalMeasurement);
    
    /* --- Devices --- */
    public const string ClientRequestDeviceList = nameof(ClientRequestDeviceList);
    public const string ServerResponseDeviceList = nameof(ServerResponseDeviceList);
    public const string BroadcastDeviceConnectionUpdate = nameof(BroadcastDeviceConnectionUpdate);
    public const string BroadcastDeviceSensorDataUpdate = nameof(BroadcastDeviceSensorDataUpdate);

    public const string AllDeviceStatus = nameof(AllDeviceStatus);
    public const string DeviceUpdateStatus = nameof(DeviceUpdateStatus);
    
    public const string GraphGetMeasurement = nameof(GraphGetMeasurement);
    public const string GraphTemperatureUpdate  = nameof(GraphTemperatureUpdate);
    public const string RequestAirQualityData = nameof(RequestAirQualityData);
}