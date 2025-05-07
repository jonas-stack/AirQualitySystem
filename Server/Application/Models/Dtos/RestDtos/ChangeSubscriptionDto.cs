namespace Application.Models.Dtos.RestDtos;

public class ChangeSubscriptionDto
{
    public string ClientId { get; set; }
    public List<string> TopicIds { get; set; }
}