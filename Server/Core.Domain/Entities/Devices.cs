namespace Core.Domain.Entities;

public class Devices
{
    
    public Guid DeviceId { get; set; }
    
    
    public string DeviceName { get; set; }
    
    
    public DateTime LastSeen { get; set; }
    
   
    public bool IsConnected { get; set; }
}