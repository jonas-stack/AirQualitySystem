using Application.Models.Dtos;
using Application.Models.Dtos.MQTT;

namespace Application.Interfaces;

public interface IDeviceService
{
    Task<List<DeviceDto>> GetAllDeviceStatus();
    Task BroadcastDeviceStatus(DeviceDto dto);
}