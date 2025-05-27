using Application.Models.Dtos.MQTT;
using Core.Domain.Entities;

namespace Application.Interfaces.Mappers;

public interface IDevicesMapper
{
    Devices MapToEntity(DeviceDto dto);
    DeviceDto MapToDto(Devices entity);
}