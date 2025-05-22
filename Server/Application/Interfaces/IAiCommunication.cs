namespace Application.Interfaces;

public interface IAiCommunication
{
    Task<string> Ai_chat(string message);
    Task<string> AnalyzeHistoricalData(string range);
    Task<string> AnalyzeLiveData();
}