namespace Application.Models.Dtos.Graph;

public class GraphModel<T> : ApplicationBaseDto
{
    public required string Identifier { get; set; }
    public required T Amount { get; set; }
    public override string EventType { get; set; }
    public override string Topic { get; set; }
}
