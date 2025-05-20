namespace Application.Models.Dtos;

public abstract class ApplicationBaseDto
{
    public abstract string EventType { get; set; }
    public abstract string Topic { get; set; }
}