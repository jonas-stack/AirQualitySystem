namespace Application.Models.Dtos.RestDtos;

public class ChangeSubscriptionDto
{
    public required string ClientId { get; set; }
    public required List<string> TopicIds { get; set; }
}