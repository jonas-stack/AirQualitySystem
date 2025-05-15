using System;
using System.Collections.Generic;

namespace Core.Domain.TestEntities;

public partial class TestDevices
{
    public Guid DeviceId { get; set; }

    public string DeviceName { get; set; } = null!;

    public bool IsConnected { get; set; }

    public DateTime LastSeen { get; set; }

    public virtual ICollection<TestDeviceConnectionHistory> TestDeviceConnectionHistory { get; set; } = new List<TestDeviceConnectionHistory>();

    public virtual ICollection<TestSensorData> TestSensorData { get; set; } = new List<TestSensorData>();
}
