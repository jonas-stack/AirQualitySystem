using Fleck;
using WebSocketBoilerplate;

namespace Application.Models.Dtos.Ai;

public class LiveAiFeedbackDto : BaseDto
{
    public required string AiAdvice { get; set; }
}