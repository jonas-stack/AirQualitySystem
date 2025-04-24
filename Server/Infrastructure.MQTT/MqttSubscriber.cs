using System.Text.Json;
using Application.Interfaces.Infrastructure.MQTT;
using Application.Models.Dtos.MqttSubscriptionDto;
using HiveMQtt.Client;
using HiveMQtt.Client.Events;
using HiveMQtt.Client.Results;
using HiveMQtt.MQTT5.ReasonCodes;
using HiveMQtt.MQTT5.Types;
using Infrastructure.MQTT.SubscriptionEventHandlers;

namespace Infrastructure.MQTT;

public class MqttSubscriber : IMqttService
{
    private readonly HiveMQClient _client;
    private readonly SensorDataHandler _dataHandler;
    private const string DefaultTopic = "AirQuality/Data";

    public MqttSubscriber(HiveMQClient client, SensorDataHandler dataHandler)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _dataHandler = dataHandler ?? throw new ArgumentNullException(nameof(dataHandler));
        SubscribeToDefaultTopic().GetAwaiter().GetResult();
    }
    
    private async Task SubscribeToDefaultTopic()
    {
        try
        {
            await SubscribeAsync(null!, DefaultTopic);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to subscribe to the default topic {DefaultTopic}.", ex);
        }
    }

    public async Task SubscribeAsync(object _, string topic)
    {
        SubscribeResult result = await _client.SubscribeAsync(topic, QualityOfService.AtLeastOnceDelivery);

        // Check if the subscription was successful
        var subscription = result.GetSubscription(topic);
        if (subscription == null || subscription.SubscribeReasonCode != SubAckReasonCode.GrantedQoS1)
        {
            throw new Exception($"Failed to subscribe to topic {topic}. Reason code: {subscription?.SubscribeReasonCode}");
        }
    }
}
