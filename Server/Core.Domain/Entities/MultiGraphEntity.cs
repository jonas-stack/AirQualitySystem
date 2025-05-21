namespace Core.Domain.Entities;

public class MultiGraphEntity
{
    public required string Timestamp { get; set; }
    public required Dictionary<string, double> Values { get; set; }
}