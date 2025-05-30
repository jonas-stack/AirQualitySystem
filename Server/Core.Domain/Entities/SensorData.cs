﻿using System;
using System.Collections.Generic;

namespace Core.Domain.Entities;

public partial class SensorData
{
    public int Id { get; set; }

    public double Temperature { get; set; }

    public double Humidity { get; set; }

    public double AirQuality { get; set; }

    public double Pm25 { get; set; }

    public Guid DeviceId { get; set; }

    public DateTime Timestamp { get; set; }
}
