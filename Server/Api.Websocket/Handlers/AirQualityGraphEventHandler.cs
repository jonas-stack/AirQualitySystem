using Fleck;
using WebSocketBoilerplate;

namespace Api.Websocket.Handlers;

// hvad klient sender
public class RequestAirQualityDataDto : BaseDto
{
    public string SomethingTheClientSends { get; set; }
}

// hvad server sender..
public class AirQualityDataGraph : BaseDto
{
    public string SomethingTheServerSends { get; set; }
}


public class AirQualityGraphEventHandler : BaseEventHandler<RequestAirQualityDataDto>
{
    public override Task Handle(RequestAirQualityDataDto dto, IWebSocketConnection socket)
    {
        throw new Exception("Not implemented");
    }
}