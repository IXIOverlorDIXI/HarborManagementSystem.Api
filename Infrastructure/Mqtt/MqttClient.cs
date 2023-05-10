using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.EnvironmentalCondition;
using Application.Interfaces;
using Application.RelativePositionMetering;
using Application.StorageEnvironmentalCondition;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Packets;

namespace Infrastructure.Mqtt
{
    public class MqttClient
    {
        private static MqttClient? _mqttClient;

        private readonly IConfiguration _config;
        private readonly ILogger<MqttClient> _logger;
        private readonly IMqttHandler _mqttRelativePositionMeteringHandler;
        private readonly IMqttHandler _mqttEnvironmentalConditionHandler;
        private readonly IMqttHandler _mqttStorageEnvironmentalConditionHandler;
        private static MqttClient _hiveMqClient;

        public MqttClient()
        {
        }

        public MqttClient(IConfiguration config,
            ILogger<MqttClient> logger,
            IServiceProvider serviceProvider)
        {
            _config = config;
            _logger = logger;

            var mqttHandlers = serviceProvider.GetServices<IMqttHandler>();

            _mqttRelativePositionMeteringHandler = mqttHandlers
                .First(o => o.GetType() == typeof(RelativePositionMeteringMqttHandler));
            _mqttEnvironmentalConditionHandler = mqttHandlers
                .First(o => o.GetType() == typeof(EnvironmentalConditionMqttHandler));
            _mqttStorageEnvironmentalConditionHandler = mqttHandlers
                .First(o => o.GetType() == typeof(StorageEnvironmentalConditionMqttHandler));

            CreateMqttClient().Wait();

            _mqttClient = this;
        }

        private async Task CreateMqttClient()
        {
            var mqttBrokerSection = _config.GetSection("MqttBroker");
            var mqttClientCredentialsSection = mqttBrokerSection.GetSection("ClientCredentials");
            var mqttTopicsSection = mqttBrokerSection.GetSection("Topics");

            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithClientId(mqttBrokerSection.GetValue<string>("ClientId"))
                .WithTcpServer(
                    mqttBrokerSection.GetValue<string>("Url"),
                    mqttBrokerSection.GetValue<int>("Port"))
                .WithCredentials(
                    mqttClientCredentialsSection.GetValue<string>("Username"), 
                    mqttClientCredentialsSection.GetValue<string>("Password"))
                .WithCleanSession()
                .WithTls()
                .Build();

            var topics = new List<MqttTopicFilter>
            {
                new MqttTopicFilter() { Topic = mqttTopicsSection.GetValue<string>("RelativePositionMetering") },
                new MqttTopicFilter() { Topic = mqttTopicsSection.GetValue<string>("EnvironmentalCondition") },
                new MqttTopicFilter() { Topic = mqttTopicsSection.GetValue<string>("StorageEnvironmentalCondition") },
            };

            var factory = new MqttFactory();
            var client = factory.CreateManagedMqttClient();

            client.ApplicationMessageReceivedAsync += async (args) =>
            {
                try
                {
                    var topic = args.ApplicationMessage.Topic;
                    var payload = Encoding.UTF8.GetString(args.ApplicationMessage.PayloadSegment);
                    
                   // _logger.LogInformation($"Received message on topic {args.ApplicationMessage.Topic}: {Encoding.UTF8.GetString(args.ApplicationMessage.Payload)}");
                    
                    KeyValuePair<bool, string> result = new KeyValuePair<bool, string>(false, "Wrong topic.");

                    if (topic.Equals(mqttTopicsSection.GetValue<string>("RelativePositionMetering")))
                    {
                        result = await _mqttRelativePositionMeteringHandler.MqttAddHandle(payload);
                    }
                    else if (topic.Equals(mqttTopicsSection.GetValue<string>("EnvironmentalCondition")))
                    {
                        result = await _mqttEnvironmentalConditionHandler.MqttAddHandle(payload);
                    }
                    else if (topic.Equals(mqttTopicsSection.GetValue<string>("StorageEnvironmentalCondition")))
                    {
                        result = await _mqttStorageEnvironmentalConditionHandler.MqttAddHandle(payload);
                    }

                    if (result.Key)
                    {
                        _logger.LogInformation(result.Value);
                    }
                    else
                    {
                        _logger.LogError(result.Value);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                }
            };

            client.ConnectedAsync += async (e) => { _logger.LogInformation("Mqtt broker connected successfully!"); };

            client.ConnectingFailedAsync += async (e) => { _logger.LogError("Mqtt broker connected unsuccessfully!"); };

            client.DisconnectedAsync += async (e) => { _logger.LogWarning("Mqtt broker disconnected!"); };

            client.ConnectionStateChangedAsync += async (e) => { _logger.LogWarning("Mqtt broker connection state changed!"); };
            
            await client.SubscribeAsync(topics);

            await client.StartAsync(
                new ManagedMqttClientOptionsBuilder()
                    .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                    .WithClientOptions(mqttClientOptions)
                    .Build());
        }
    }
}