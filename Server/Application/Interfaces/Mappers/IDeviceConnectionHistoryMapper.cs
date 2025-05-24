using Application.Models.Dtos;
using Core.Domain.Entities;

namespace Application.Interfaces.Mappers;

public interface IDeviceConnectionHistoryMapper
{
    DeviceConnectionHistoryDto MapToDto(DeviceConnectionHistory deviceConnectionHistory);
    List<DeviceConnectionHistoryDto> MapWithDurationAndFilter(List<DeviceConnectionHistory> history);
}