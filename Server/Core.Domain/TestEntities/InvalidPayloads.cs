using System;
using System.Collections.Generic;

namespace Core.Domain.Entities;

public partial class InvalidPayloads
{
    public int Id { get; set; }

    public string? DeviceId { get; set; }

    public string RawPayload { get; set; } = null!;

    public string ErrorReason { get; set; } = null!;

    public DateTime Timestamp { get; set; }
}
