using Application.Enums;
using Application.Interfaces;
using Application.Models.Dtos.Graph;
using Fleck;
using WebSocketBoilerplate;

namespace Api.Websocket.Handlers;

// hvad klient sender
public class RequestAirQualityData : BaseDto
{
    public GraphType GraphType { get; set; }
    public TimePeriod TimePeriod { get; set; }
}

// hvad server sender..
public class AirQualityDataGraph<T> : BaseDto
{
    public GraphType GraphType { get; set; }
    public TimePeriod TimePeriod { get; set; }
    public List<GraphModel<T>> Data { get; set; }
}

public class AirQualityGraphEventHandler(IGraphService graphService) : BaseEventHandler<RequestAirQualityData>
{
    public override Task Handle(RequestAirQualityData dto, IWebSocketConnection socket)
    {
        var result = graphService.GetGraph(dto.GraphType, dto.TimePeriod);
        throw new Exception("Not implemented");
    }
}