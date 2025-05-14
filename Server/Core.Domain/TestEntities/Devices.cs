namespace Core.Domain.TestEntities;

public partial class Devices
{
    public Guid DeviceId { get; set; }

    public string DeviceName { get; set; } = null!;

    public bool IsConnected { get; set; }

    public DateTime LastSeen { get; set; }

    public virtual ICollection<DeviceConnectionHistory> DeviceConnectionHistory { get; set; } = new List<DeviceConnectionHistory>();
}
