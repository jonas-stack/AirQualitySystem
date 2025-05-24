using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;

namespace Application.Services;

public class SensorDataService(IDeviceRepository deviceRepository) : ISensorDataService
{
    
}