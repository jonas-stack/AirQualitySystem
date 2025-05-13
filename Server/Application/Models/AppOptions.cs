using System.ComponentModel.DataAnnotations;

namespace Application.Models;

public sealed class AppOptions
{
    [Required] public string JwtSecret { get; set; } = string.Empty!;
    [Required] public string DbConnectionString { get; set; } = string.Empty!;
    public bool Seed { get; set; } = true;
    public int PORT { get; set; } = 8080;
    public int WS_PORT { get; set; } = 8181;
    public int REST_PORT { get; set; } = 5000;
    [Required] public string MQTT_BROKER_HOST { get; set; } = null!;
    [Required] public string MQTT_USERNAME { get; set; } = null!;
    [Required] public string MQTT_PASSWORD { get; set; } = null!;
}