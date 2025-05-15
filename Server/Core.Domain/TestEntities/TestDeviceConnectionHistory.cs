using System;
using System.Collections.Generic;

namespace Core.Domain.TestEntities;

public partial class TestDeviceConnectionHistory
{
    public int Id { get; set; }

    public Guid DeviceId { get; set; }

    public bool IsConnected { get; set; }

    public DateTime LastSeen { get; set; }

    public virtual TestDevices Device { get; set; } = null!;
}
