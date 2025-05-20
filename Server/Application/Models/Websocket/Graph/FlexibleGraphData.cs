namespace Application.Models.Websocket.Graph;

public class FlexibleGraphData
{
    public string Time { get; set; }
    public Dictionary<string, double> DataPoints { get; set; }
}