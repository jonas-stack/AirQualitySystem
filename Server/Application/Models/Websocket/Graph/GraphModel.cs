namespace Application.Models.Dtos.Graph;

public class GraphModel<T>
{
    public required string Identifier { get; set; }
    public required T Amount { get; set; }
}
