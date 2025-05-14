namespace Application.Models.Dtos.Graph;

public class GraphModel<T> : ApplicationBaseDto
{
    public required string Name { get; set; }
    public required T Amount { get; set; }
    public override string eventType { get; set; }
}
