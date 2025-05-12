using System.Collections.Concurrent;
using System.Text.Json;
using Fleck;
using Microsoft.Extensions.Logging;
using Application.Interfaces.Infrastructure.Websocket;

namespace Infrastructure.Websocket;

public sealed class WebSocketConnectionManager : IConnectionManager
{
    private readonly ILogger<WebSocketConnectionManager> _logger;
    private readonly ConcurrentDictionary<string, IWebSocketConnection> _connectionIdToSocket = new();
    private readonly ConcurrentDictionary<string, HashSet<string>> _topicMembers = new();
    private readonly ConcurrentDictionary<string, HashSet<string>> _memberTopics = new();
    private readonly ConcurrentDictionary<string, string> _socketToConnectionId = new();
        public WebSocketConnectionManager(ILogger<WebSocketConnectionManager> logger)
        {
            _logger = logger;
        }


        public ConcurrentDictionary<string, object> GetConnectionIdToSocketDictionary()
        {
            var idToSocket = new ConcurrentDictionary<string, object>();
            foreach (var (key, value) in _connectionIdToSocket)
            {
                idToSocket.TryAdd(key, value);
            }

            return idToSocket;
        }

        public ConcurrentDictionary<string, string> GetSocketIdToClientIdDictionary()
        {
            return _socketToConnectionId;
        }

        public async Task OnOpen(object socket, string clientId)
    {
        if (socket is not IWebSocketConnection webSocket)
        {
            throw new ArgumentException("Socket must be an IWebSocketConnection", nameof(socket));
        }

        _logger.LogDebug("OnOpen called with clientId: {ClientId}", clientId);

        // Remove any existing connection for this client
        if (_connectionIdToSocket.TryRemove(clientId, out var oldSocket))
        {
            _socketToConnectionId.TryRemove(oldSocket.ConnectionInfo.Id.ToString(), out _);
            _logger.LogInformation("Removed old connection {SocketId} for client {ClientId}", 
                oldSocket.ConnectionInfo.Id, clientId);
        }

        // Add new connection
        _connectionIdToSocket[clientId] = webSocket;
        _socketToConnectionId[webSocket.ConnectionInfo.Id.ToString()] = clientId;

        // Resubscribe to previous topics if any
        if (_memberTopics.TryGetValue(clientId, out var topics))
        {
            foreach (var topic in topics)
            {
                await AddToTopic(topic, clientId);
            }
        }

        await LogCurrentState();
    }

    public async Task OnClose(object socket, string clientId)
    {
        if (socket is not IWebSocketConnection webSocket)
        {
            throw new ArgumentException("Socket must be an IWebSocketConnection", nameof(socket));
        }

        var socketId = webSocket.ConnectionInfo.Id.ToString();
        _logger.LogDebug("OnClose called with clientId: {ClientId} and socketId: {SocketId}", clientId, socketId);

        // Remove socket mappings
        if (_connectionIdToSocket.TryGetValue(clientId, out var currentSocket) &&
            currentSocket.ConnectionInfo.Id.ToString() == socketId)
        {
            _connectionIdToSocket.TryRemove(clientId, out _);
        }
        _socketToConnectionId.TryRemove(socketId, out _);

        // Note: We don't remove topic subscriptions on disconnect to allow for reconnection
        await LogCurrentState();
    }

    public async Task AddToTopic(string topic, string memberId)
    {
        _topicMembers.AddOrUpdate(
            topic,
            new HashSet<string> { memberId },
            (_, existing) =>
            {
                var newSet = new HashSet<string>(existing);
                newSet.Add(memberId);
                return newSet;
            });

        _memberTopics.AddOrUpdate(
            memberId,
            new HashSet<string> { topic },
            (_, existing) =>
            {
                var newSet = new HashSet<string>(existing);
                newSet.Add(topic);
                return newSet;
            });

        _logger.LogInformation("Added to topic {Topic} for client {ClientId}", topic, memberId);
        await LogCurrentState();
    }

    public async Task RemoveFromTopic(string topic, string memberId)
    {
        _topicMembers.AddOrUpdate(
            topic,
            new HashSet<string>(),
            (_, existing) =>
            {
                var newSet = new HashSet<string>(existing);
                newSet.Remove(memberId);
                return newSet;
            });

        if (_topicMembers.TryGetValue(topic, out var members) && !members.Any())
        {
            _topicMembers.TryRemove(topic, out _);
        }

        _memberTopics.AddOrUpdate(
            memberId,
            new HashSet<string>(),
            (_, existing) =>
            {
                var newSet = new HashSet<string>(existing);
                newSet.Remove(topic);
                return newSet;
            });

        if (_memberTopics.TryGetValue(memberId, out var topics) && !topics.Any())
        {
            _memberTopics.TryRemove(memberId, out _);
        }

        await LogCurrentState();
    }

    public async Task BroadcastToTopic<TMessage>(string topic, TMessage message) where TMessage : class
    {
        if (!_topicMembers.TryGetValue(topic, out var members))
        {
            _logger.LogWarning("No topic found: {Topic}", topic);
            return;
        }

        var membersList = members.ToList();
        foreach (var memberId in membersList)
        {
            if (!_connectionIdToSocket.TryGetValue(memberId, out var socket))
            {
                continue; // Skip offline members but don't remove their subscription
            }

            if (!socket.IsAvailable)
            {
                continue; // Skip unavailable sockets but don't remove their subscription
            }

            try
            {
                var json = JsonSerializer.Serialize(message, 
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                await socket.Send(json);
                _logger.LogDebug("Sent message to client {ClientId} on topic {Topic}", memberId, topic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message to client {ClientId} on topic {Topic}", memberId, topic);
            }
        }
    }

    public Task<List<string>> GetMembersFromTopicId(string topic)
    {
        return Task.FromResult(
            _topicMembers.TryGetValue(topic, out var members)
                ? members.ToList()
                : new List<string>());
    }

    public Task<List<string>> GetTopicsFromMemberId(string memberId)
    {
        return Task.FromResult(
            _memberTopics.TryGetValue(memberId, out var topics)
                ? topics.ToList()
                : new List<string>());
    }

    public string GetClientIdFromSocket(object socket)
    {
        if (socket is not IWebSocketConnection webSocket)
        {
            throw new ArgumentException("Socket must be an IWebSocketConnection", nameof(socket));
        }

        if (_socketToConnectionId.TryGetValue(webSocket.ConnectionInfo.Id.ToString(), out var clientId))
        {
            return clientId;
        }
        throw new Exception("Could not find clientId for socket: "+webSocket.ConnectionInfo.Id);
    }

    public object GetSocketFromClientId(string clientId)
    {
        if (_connectionIdToSocket.TryGetValue(clientId, out var socket))
        {
            return socket;
        }
        throw new Exception("Could not find socket for clientId: "+clientId);
    }

    private async Task LogCurrentState()
    {
        try
        {
            var state = new
            {
                Connections = _connectionIdToSocket.Keys,
                Topics = _topicMembers,
                MemberSubscriptions = _memberTopics
            };

            _logger.LogDebug("Current state: {State}", 
                JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging current state");
        }
    }
}