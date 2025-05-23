namespace Core.Domain.Entities;

// eftersom vi ikke vil bruge vores model i repository
public class GraphEntity<T>
{
    public required T Amount { get; set; }
    public required string Timestamp { get; set; }
}