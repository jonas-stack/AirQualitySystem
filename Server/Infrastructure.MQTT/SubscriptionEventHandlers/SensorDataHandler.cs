using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Application.Models.Dtos.MqttSubscriptionDto;
using Core.Domain.Entities;
using HiveMQtt.Client.Events;
using HiveMQtt.MQTT5.Types;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.Extensions.Logging;

namespace Infrastructure.MQTT.SubscriptionEventHandlers;

public class SensorDataHandler : IMqttMessageHandler
{
    private readonly ILogger<SensorDataHandler> _logger;
    private readonly MyDbContext _dbContext;

    public SensorDataHandler(ILogger<SensorDataHandler> logger, MyDbContext dbContext)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public string TopicFilter { get; } = "sensors/+/data";
    public QualityOfService QoS { get; } = QualityOfService.AtLeastOnceDelivery;

    public void Handle(object? sender, OnMessageReceivedEventArgs args)
    {
        try
        {
            // Deserialize the payload into a DTO
            var dto = JsonSerializer.Deserialize<SensorDataDto>(args.PublishMessage.PayloadAsString,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? throw new Exception("Could not deserialize into " + nameof(SensorDataDto));

            // Validate the DTO
            var context = new ValidationContext(dto);
            Validator.ValidateObject(dto, context);
            
            // map dto to entity
            var entity = new SensorData
            {
                Temperature = dto.Temperature,
                Humidity = dto.Humidity,
                AirQuality = dto.AirQuality,
                Pm25 = dto.Pm25,
                DeviceId = dto.DeviceId,
                Timestamp = dto.Timestamp
            };
            
            // Save entity to the database
            _dbContext.SensorData.Add(entity);
            _dbContext.SaveChanges();

            _logger.LogInformation("Sensor data saved successfully: {Data}", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing sensor data message.");
        }
    }
}