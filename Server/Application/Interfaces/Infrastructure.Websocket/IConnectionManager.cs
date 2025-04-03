using System.Collections.Concurrent;

namespace Application.Interfaces.Infrastructure.Websocket;

public interface IConnectionManager
{
    public ConcurrentDictionary<string, object> GetConnectionIdToSocketDictionary();
    public ConcurrentDictionary<string, string> GetSocketIdToClientIdDictionary();

    Task OnOpen(object socket, string clientId);
    Task OnClose(object socket, string clientId);
    Task AddToTopic(string topic, string memberId);
    Task RemoveFromTopic(string topic, string memberId);
    Task BroadcastToTopic<TMessage>(string topic, TMessage message) where TMessage : class;
    Task<List<string>> GetMembersFromTopicId(string topic);
    Task<List<string>> GetTopicsFromMemberId(string memberId);
    public string GetClientIdFromSocket(object socket);
    public object GetSocketFromClientId(string clientId);
}