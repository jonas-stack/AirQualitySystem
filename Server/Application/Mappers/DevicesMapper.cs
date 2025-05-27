using Application.Interfaces.Mappers;
using Application.Models.Dtos.MQTT;
using Application.Utility;
using Core.Domain.Entities;

namespace Application.Mappers;

public class DevicesMapper : IDevicesMapper
{
    public Devices MapToEntity(DeviceDto dto)
    {
        return new Devices
        {
            DeviceId = DataTypeConverter.GetDeviceGuid(dto.DeviceName),
            DeviceName = dto.DeviceName,
            IsConnected = dto.IsConnected,
            LastSeen = DataTypeConverter.GetLocalDateTime(dto.LastSeen),
            Updateinterval = dto.UpdateInterval
        };
    }

    public DeviceDto MapToDto(Devices entity)
    {
        return new DeviceDto
        {
            DeviceGuid = entity.DeviceId.ToString(),
            DeviceName = entity.DeviceName,
            IsConnected = entity.IsConnected,
            LastSeen = entity.LastSeen.Ticks,
            UpdateInterval = entity.Updateinterval
        };
    }
}