﻿using System;
using System.Collections.Generic;

namespace Core.Domain.Entities;

public partial class Devices
{
    public Guid DeviceId { get; set; }

    public string DeviceName { get; set; } = null!;

    public bool IsConnected { get; set; }

    public DateTime LastSeen { get; set; }

    public int Updateinterval { get; set; }
}
