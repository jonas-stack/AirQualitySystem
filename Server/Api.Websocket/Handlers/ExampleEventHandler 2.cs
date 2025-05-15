using Fleck;
using WebSocketBoilerplate;

namespace Api.Websocket.Handlers;

/// <summary>
///     on the BaseDto there is eventType string so no need to define it here
///     but when you want to send this object it should look like:
///     {
///     "eventType": "ExampleClientDto",
///     "SomethingTheClientSends": "hello world"
///     }
/// </summary>
public class ExampleClientDto : BaseDto
{
    public string SomethingTheClientSends { get; set; }
}

public class ExampleServerResponse : BaseDto
{
    public string SomethingTheServerSends { get; set; }
}

public class ExampleEventHandler : BaseEventHandler<ExampleClientDto>
{
    public override Task Handle(ExampleClientDto dto, IWebSocketConnection socket)
    {
        //you can access the dto like: dto.SomethingTheClientSends; 

        //when sending response to the Typescript client using sendRequest, remember to attach requestId like below: (not for broadcasts)
        socket.SendDto(new ExampleServerResponse { SomethingTheServerSends = "hi you", requestId = dto.requestId });

        throw new Exception("This will be caught by the global exception handler and returned to the client");
    }
}