using Application.Models.Dtos.MQTT;
using Application.Utility;
using Core.Domain.Entities;
using Core.Domain.TestEntities;

namespace Application.Mappers;

public class DevicesMapper
{
    public TestDevices MapToTestEntity(DeviceDto dto)
    {
        return new TestDevices
        {
            DeviceId = DataTypeConverter.GetDeviceGuid(dto.DeviceName),
            DeviceName = dto.DeviceName,
            IsConnected = dto.IsConnected,
            LastSeen = DataTypeConverter.GetLocalDateTime(dto.LastSeen)
        };
    }
    
    public Devices MapToProductionEntity(DeviceDto dto)
    {
        return new Devices()
        {
            DeviceId = DataTypeConverter.GetDeviceGuid(dto.DeviceName),
            DeviceName = dto.DeviceName,
            IsConnected = dto.IsConnected,
            LastSeen = DataTypeConverter.GetLocalDateTime(dto.LastSeen)
        };
    }
}