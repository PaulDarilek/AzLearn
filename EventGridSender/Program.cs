namespace EventGridSender
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Azure.EventHubs;
    using Microsoft.Extensions.Configuration;

    public class Program
    {
        private static readonly IDictionary<string, string> _settings = GetConfiguration();
        private static readonly string EventHubConnectionString = _settings[nameof(EventHubConnectionString)];
        private static readonly string EventHubName = _settings[nameof(EventHubName)];

        private static EventHubClient eventHubClient;

        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            // Creates an EventHubsConnectionStringBuilder object from the connection string, and sets the EntityPath.
            // Typically, the connection string should have the entity path in it, but for the sake of this simple scenario
            // we are using the connection string from the namespace.
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(EventHubConnectionString)
            {
                EntityPath = EventHubName
            };

            eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            await SendMessagesToEventHub(100);

            await eventHubClient.CloseAsync();

            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();
        }

        // Uses the event hub client to send 100 messages to the event hub.
        private static async Task SendMessagesToEventHub(int numMessagesToSend)
        {
            for (var i = 0; i < numMessagesToSend; i++)
            {
                try
                {
                    var message = $"Message {i}";
                    Console.WriteLine($"Sending message: {message}");
                    await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"{DateTime.Now} > Exception: {exception.Message}");
                }

                await Task.Delay(10);
            }

            Console.WriteLine($"{numMessagesToSend} messages sent.");
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