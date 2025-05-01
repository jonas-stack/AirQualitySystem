namespace Core.Domain.TestEntities;

public partial class Devicestatus
{
    public int Id { get; set; }

    public string Status { get; set; } = null!;

    public string Deviceid { get; set; } = null!;

    public DateTime Timestamp { get; set; }
}
