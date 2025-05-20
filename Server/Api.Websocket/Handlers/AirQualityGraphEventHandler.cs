using Fleck;
using WebSocketBoilerplate;

namespace Api.Websocket.Handlers;

// hvad klient sender
public class RequestAirQualityData : BaseDto
{
    public string SomethingTheClientSends { get; set; }
}

// hvad server sender..
public class AirQualityDataGraph : BaseDto
{
    public string SomethingTheServerSends { get; set; }
}

public class AirQualityGraphEventHandler : BaseEventHandler<RequestAirQualityData>
{
    public override Task Handle(RequestAirQualityData dto, IWebSocketConnection socket)
    {
        throw new Exception("Not implemented");
    }
}