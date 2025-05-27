using System;
using System.Collections.Generic;

namespace Core.Domain.Entities;

public partial class Devices
{
    public Guid DeviceId { get; set; }

    public string DeviceName { get; set; } = null!;

    public bool IsConnected { get; set; }

    public DateTime LastSeen { get; set; }

    public virtual ICollection<DeviceConnectionHistory> DeviceConnectionHistory { get; set; } = new List<DeviceConnectionHistory>();

    public virtual ICollection<SensorData> SensorData { get; set; } = new List<SensorData>();
}
