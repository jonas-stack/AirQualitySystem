namespace Application.Interfaces.Infrastructure.Ai;

public interface IAiService
{
    Task<string> Ai_chat(string message);
}