using Core.Domain.Entities;

namespace Core.Domain.TestEntities;

public partial class TestDevices
{
    public Guid DeviceId { get; set; }

    public string DeviceName { get; set; } = null!;

    public bool IsConnected { get; set; }

    public DateTime LastSeen { get; set; }

    public virtual ICollection<TestDeviceConnection> TestDeviceConnection { get; set; } = new List<TestDeviceConnection>();

    public virtual ICollection<TestSensorData> TestSensorData { get; set; } = new List<TestSensorData>();
}
