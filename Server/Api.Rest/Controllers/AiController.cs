using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rest.Controllers;

public class MessageFromClient
{
    public required string Message { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class AiController(IAiCommunication aiCommunication) : ControllerBase
{
    [HttpGet]
    [Route("chat")]
    public async Task<ActionResult> Subscribe([FromHeader] string authorization, [FromBody] MessageFromClient message)
    {
        var response = await aiCommunication.Ai_chat(message.Message);
        return Ok(response);
    }
}