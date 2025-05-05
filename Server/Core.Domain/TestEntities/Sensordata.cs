namespace Core.Domain.TestEntities;

public partial class Sensordata
{
    public int Id { get; set; }

    public double Temperature { get; set; }

    public double Humidity { get; set; }

    public double Airquality { get; set; }

    public double Pm25 { get; set; }

    public string Deviceid { get; set; } = null!;

    public DateTime Timestamp { get; set; }
}
