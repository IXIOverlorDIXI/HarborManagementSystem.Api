using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.EnvironmentalCondition;
using Application.Interfaces;
using Application.RelativePositionMetering;
using Application.StorageEnvironmentalCondition;
using HiveMQtt.Client;
using HiveMQtt.Client.Options;
using HiveMQtt.MQTT5.ReasonCodes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Application.Mqtt
{
    public class MqttClient
    {
        private static MqttClient? _mqttClient;

        private readonly IConfiguration _config;
        private readonly ILogger<MqttClient> _logger;
        private readonly IMqttHandler _mqttRelativePositionMeteringHandler;
        private readonly IMqttHandler _mqttEnvironmentalConditionHandler;
        private readonly IMqttHandler _mqttStorageEnvironmentalConditionHandler;
        private static HiveMQClient _hiveMqClient;

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
            var mqttClientOptions = new HiveMQClientOptions();

            var mqttBrokerSection = _config.GetSection("MqttBroker");
            var mqttClientCredentialsSection = mqttBrokerSection.GetSection("ClientCredentials");
            var mqttTopicsSection = mqttBrokerSection.GetSection("Topics");

            mqttClientOptions.Host = mqttBrokerSection.GetValue<string>("Url");
            mqttClientOptions.Port = mqttBrokerSection.GetValue<int>("Port");
            mqttClientOptions.UserName = mqttClientCredentialsSection.GetValue<string>("Username");
            mqttClientOptions.Password = mqttClientCredentialsSection.GetValue<string>("Password");
            mqttClientOptions.UseTLS = true;

            _hiveMqClient = new HiveMQClient(mqttClientOptions);

            var connectResult = await _hiveMqClient.ConnectAsync().ConfigureAwait(false);

            if (connectResult.ReasonCode == ConnAckReasonCode.Success)
            {
                _logger.LogInformation("Mqtt broker connected successfully!");
            }
            else
            {
                _logger.LogError("Mqtt broker connected unsuccessfully!");
            }

            _hiveMqClient.OnMessageReceived += async (sender, args) =>
            {
                try
                {
                    var topic = args.PublishMessage.Topic;
                    var payload = args.PublishMessage.PayloadAsString;
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

            await _hiveMqClient.SubscribeAsync(mqttTopicsSection.GetValue<string>("RelativePositionMetering"))
                .ConfigureAwait(false);
            await _hiveMqClient.SubscribeAsync(mqttTopicsSection.GetValue<string>("EnvironmentalCondition"))
                .ConfigureAwait(false);
            await _hiveMqClient.SubscribeAsync(mqttTopicsSection.GetValue<string>("StorageEnvironmentalCondition"))
                .ConfigureAwait(false);
        }
    }
}