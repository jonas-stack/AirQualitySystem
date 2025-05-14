using Fleck;
using WebSocketBoilerplate;

namespace Api.Websocket.Handlers;

public class RequestAirQualityDataDto : BaseDto
{
    public string SomethingTheClientSends { get; set; }
}

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