namespace Application.Models.Websocket.Graph;

public class GraphModel
{
    public string Label { get; set; }
    public List<DataPoint> Data { get; set; }
}

public class DataPoint
{
    public string X { get; set; }
    public double Y { get; set; }
}
