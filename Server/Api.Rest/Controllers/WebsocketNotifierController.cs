using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rest.Controllers;

public class NotifyMessageDto
{
    public required string Topic { get; set; }
    public required String Message { get; set; }
}

[ApiController]
public class WebsocketNotifierController(IWebsocketNotifierService websocketNotifierService) : ControllerBase
{
    public const string NotifyRoute = nameof(NotifyRoute);
    
    [HttpPost]
    [Route(NotifyRoute)]
    public async Task<ActionResult> Notify([FromBody] NotifyMessageDto dto)
    {
        await websocketNotifierService.NotifyAsync(dto.Topic, dto.Message);
        return Ok();
    }
    
}