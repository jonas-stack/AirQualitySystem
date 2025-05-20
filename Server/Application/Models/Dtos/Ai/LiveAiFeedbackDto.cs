using Fleck;
using WebSocketBoilerplate;

namespace Application.Models.Dtos.Ai;

public class LiveAiFeedbackDto : BaseDto
{
    public string AiAdvice { get; set; }
}