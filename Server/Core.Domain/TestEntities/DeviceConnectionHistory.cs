using Core.Domain.Entities;

namespace Core.Domain.TestEntities;

public partial class DeviceConnectionHistory
{
    public int Id { get; set; }

    public Guid DeviceId { get; set; }

    public bool IsConnected { get; set; }

    public DateTime Timestamp { get; set; }

    public virtual Devices Device { get; set; } = null!;
}
