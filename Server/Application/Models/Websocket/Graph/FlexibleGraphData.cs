namespace Application.Models.Websocket.Graph;

public class FlexibleGraphData
{
    public required string Time { get; set; }
    public required Dictionary<string, double> DataPoints { get; set; }
}