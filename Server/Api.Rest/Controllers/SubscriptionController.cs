using Application.Interfaces;
using Application.Models.Dtos.RestDtos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rest.Controllers;

[ApiController]
public class SubscriptionController(
    IWebsocketSubscriptionService websocketSubscriptionService) : ControllerBase
{
    public const string SubscriptionRoute = nameof(Subscribe);

    public const string UnsubscribeRoute = nameof(Unsubscribe);
    
    [HttpPost]
    [Route(SubscriptionRoute)]
    public async Task<ActionResult> Subscribe([FromHeader] string authorization, [FromBody] ChangeSubscriptionDto dto)
    {
        await websocketSubscriptionService.SubscribeToTopic(dto.ClientId, dto.TopicIds);
        return Ok();
    }

    [HttpPost]
    [Route(UnsubscribeRoute)]
    public async Task<ActionResult> Unsubscribe([FromHeader] string authorization, [FromBody] ChangeSubscriptionDto dto)
    {
        await websocketSubscriptionService.UnsubscribeFromTopic(dto.ClientId, dto.TopicIds);
        return Ok();
    }
}