using Application.Enums;
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
public class AirQualityDataGraph : BaseDto
{
    public GraphType GraphType { get; set; }
    public TimePeriod TimePeriod { get; set; }
}

public class AirQualityGraphEventHandler : BaseEventHandler<RequestAirQualityData>
{
    public override Task Handle(RequestAirQualityData dto, IWebSocketConnection socket)
    {
        throw new Exception("Not implemented");
    }
}