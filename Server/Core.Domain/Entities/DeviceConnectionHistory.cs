using System;
using System.Collections.Generic;

namespace Core.Domain.Entities;

public partial class DeviceConnectionHistory
{
    public int Id { get; set; }

    public Guid DeviceId { get; set; }

    public bool IsConnected { get; set; }

    public DateTime LastSeen { get; set; }
}
