using Application.Interfaces.Mappers;
using Application.Models.Dtos;
using Core.Domain.Entities;

namespace Application.Mappers;

public class DeviceConnectionHistoryMapper : IDeviceConnectionHistoryMapper
{
    public DeviceConnectionHistoryDto MapToDto(DeviceConnectionHistory deviceConnectionHistory)
    {
        return new DeviceConnectionHistoryDto
        {
            Id = deviceConnectionHistory.Id,
            DeviceId = deviceConnectionHistory.DeviceId.ToString(),
            IsConnected = deviceConnectionHistory.IsConnected,
            LastSeen = deviceConnectionHistory.LastSeen.Ticks,
        };
    }
}