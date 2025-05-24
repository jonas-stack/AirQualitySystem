// Path: Server/Application/Interfaces/Mappers/ISensorDataMapper.cs
using Application.Models.Dtos.MQTT;
using Core.Domain.Entities;

namespace Application.Interfaces.Mappers;

public interface ISensorDataMapper
{
    SensorData MapToEntity(SensorDataDto dto);
    SensorDataDto MapToDto(SensorData entity);
}