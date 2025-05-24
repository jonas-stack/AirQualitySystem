using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Interfaces.Infrastructure.Websocket;
using Application.Models.Dtos;
using Application.Models.Dtos.MQTT;
using Application.Models.Websocket;

namespace Application.Services;

public class SensorDataService(IDeviceRepository deviceRepository, ISensorDataRepository sensorDataRepository, IConnectionManager connectionManager) : ISensorDataService
{
    public async Task Broadcast(SensorDataDto sensorData)
    {
        BroadcastSensorData(sensorData);
    }

    private async void BroadcastSensorData(SensorDataDto sensorData)
    {
        sensorData.Temperature = Math.Round(sensorData.Temperature, 2);
        sensorData.AirQuality = Math.Round(sensorData.AirQuality, 2);
        sensorData.Humidity = Math.Round(sensorData.Humidity, 2);
        
        var response = new WebsocketMessage<SensorDataDto>
        {
            Topic = WebsocketTopics.Device,
            eventType = WebsocketEvents.BroadcastDeviceSensorDataUpdate,
            Data = sensorData
        };
        
        await connectionManager.BroadcastToTopic(WebsocketTopics.Device, response);
    }
    
    public async Task<PagedResult<SensorDataDto>> GetSensorDataForDeviceAsync(string deviceId, int pageNumber = 1, int pageSize = 50)
    {
        // den her thrower hvis fejl, så ingen grund til null checks
        await deviceRepository.GetDevice(deviceId);
        return await sensorDataRepository.GetSensorDataForDeviceAsync(deviceId, pageNumber, pageSize);
    }
}