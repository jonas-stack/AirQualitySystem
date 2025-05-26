using Application.Models.Dtos;
using Application.Models.Dtos.MQTT;
using Core.Domain.Entities;

namespace Application.Interfaces.Infrastructure.Postgres;

public interface IDeviceRepository
{
    
    public Task SaveDevicesAsync(Devices devices);
    public Task<bool> DeviceExistsAsync(Guid deviceId);
    public Task RegisterNewDeviceAsync(Guid deviceId, string deviceName, DateTime lastSeen);
    public Task<List<DeviceDto>> GetAllDevices();
    public Task<DeviceDto> GetDeviceStatus();

    Task<DeviceDto> GetDevice(string deviceId);
    Task<PagedResult<DeviceConnectionHistory>> GetDeviceConnectionHistoryAsync(string deviceId, int pageNumber, int pageSize);
    DeviceStatsDto GetStats();
}