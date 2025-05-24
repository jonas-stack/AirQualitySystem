using Application.Models.Dtos;
using Application.Models.Dtos.MQTT;
using Core.Domain.Entities;

namespace Application.Interfaces;

public interface IDeviceService
{
    Task<List<DeviceDto>> GetAllDeviceStatus();
    Task BroadcastDeviceStatus(DeviceDto dto);
    Task<DeviceDto> GetDeviceStatus();
    Task BroadcastData(DeviceDto entity);
    Task<PagedResult<DeviceConnectionHistoryDto>> GetDeviceHistory(string dtoDeviceId, int dtoPageNumber, int dtoPageSize);
}