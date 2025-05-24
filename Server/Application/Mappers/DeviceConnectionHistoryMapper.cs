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
    
    public List<DeviceConnectionHistoryDto> MapWithDurationAndFilter(List<DeviceConnectionHistory> history)
    {
        var ordered = history.OrderByDescending(h => h.LastSeen).ToList();

        var result = new List<DeviceConnectionHistoryDto>();
        DeviceConnectionHistory? lastDisconnect = null;
        bool lastWasConnected = false;

        foreach (var entry in ordered)
        {
            if (entry.IsConnected)
            {
                if (lastWasConnected)
                    continue;

                var dto = new DeviceConnectionHistoryDto
                {
                    Id = entry.Id,
                    DeviceId = entry.DeviceId.ToString(),
                    IsConnected = true,
                    LastSeen = entry.LastSeen.Ticks,
                    Duration = lastDisconnect != null
                        ? (long)(entry.LastSeen - lastDisconnect.LastSeen).TotalSeconds
                        : null
                };

                result.Add(dto);
                lastWasConnected = true;
                lastDisconnect = null;
            }
            else
            {
                var dto = new DeviceConnectionHistoryDto
                {
                    Id = entry.Id,
                    DeviceId = entry.DeviceId.ToString(),
                    IsConnected = false,
                    LastSeen = entry.LastSeen.Ticks
                };

                result.Add(dto);
                lastDisconnect = entry;
                lastWasConnected = false;
            }
        }

        return result;
    }
}