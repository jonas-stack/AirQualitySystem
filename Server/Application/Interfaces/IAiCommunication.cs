namespace Application.Interfaces;

public interface IAiCommunication
{
    Task<string> Ai_chat(string message);
}