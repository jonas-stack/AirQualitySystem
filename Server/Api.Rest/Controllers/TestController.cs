using Application.Interfaces;
using Application.Models.Dtos.MQTT;
using Application.Models.Dtos.RestDtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController(
    IDeviceService deviceService) : ControllerBase
{
    [HttpPost]
    [Route("Test")]
    public async Task<ActionResult> Subscribe([FromBody] DeviceIntervalUpdateDto dto)
    {
        await deviceService.UpdateDeviceInterval(dto);
        return Ok();
    }
}