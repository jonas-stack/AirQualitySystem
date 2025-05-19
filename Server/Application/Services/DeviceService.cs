using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Models.Dtos;
using Application.Models.Dtos.MQTT;

namespace Application.Services;

public class DeviceService : IDeviceService {
    
    private readonly IDeviceRepository _deviceRepository;
    
    public DeviceService(IDeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }
    
    public Task<List<DeviceDto>> GetAllDeviceStatus()
    {
        return _deviceRepository.GetAllDevices();
    }
}