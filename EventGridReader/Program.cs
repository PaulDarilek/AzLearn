using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventGridReader
{

    public class Program
    {
        private static readonly IDictionary<string, string> _settings = GetConfiguration();
        private static readonly string EventHubConnectionString = _settings[nameof(EventHubConnectionString)];
        private static readonly string EventHubName = _settings[nameof(EventHubName)];

        private static readonly string StorageContainerName = _settings[nameof(StorageContainerName)];
        private static readonly string StorageAccountName = _settings[nameof(StorageAccountName)];
        private static readonly string StorageAccountKey = _settings[nameof(StorageAccountKey)];

        private static readonly string StorageConnectionString = $"DefaultEndpointsProtocol=https;AccountName={StorageAccountName};AccountKey={StorageAccountKey}";

        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            Console.WriteLine("Registering EventProcessor...");

            var eventProcessorHost = new EventProcessorHost(
                EventHubName,
                PartitionReceiver.DefaultConsumerGroupName,
                EventHubConnectionString,
                StorageConnectionString,
                StorageContainerName);

            // Registers the Event Processor Host and starts receiving messages
            await eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor>();

            Console.WriteLine("Receiving. Press ENTER to stop worker.");
            Console.ReadLine();

            // Disposes of the Event Processor Host
            await eventProcessorHost.UnregisterEventProcessorAsync();
        }

        private static IDictionary<string, string> GetConfiguration()
        {
            var builder = new ConfigurationBuilder();

            // add User secrets for development values.
            _ = builder.AddUserSecrets(typeof(Program).Assembly);

            var config = builder.Build();
            var dict = new Dictionary<string, string>(config.AsEnumerable());
            return dict;
        }
    }
}