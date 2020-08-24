using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace QueueApp
{
    class Program
    {
        private static readonly IDictionary<string, string> _settings = GetConfiguration();
        private static readonly string StorageQueueConnection = _settings[nameof(StorageQueueConnection)];

        static async Task Main(string[] args)
        {
            if (args.Length > 0)
            {
                string value = string.Join(" ", args);
                SendArticleAsync(value).Wait();
                Console.WriteLine($"Sent: {value}");
            }
            else
            {
                string value = await ReceiveArticleAsync();
                Console.WriteLine($"Received {value}");
            }
        }

        static async Task SendArticleAsync(string newsMessage)
        {
            CloudQueue queue = GetQueue();
            bool createdQueue = await queue.CreateIfNotExistsAsync();
            if (createdQueue)
            {
                Console.WriteLine("The queue of news articles was created.");
            }

            CloudQueueMessage articleMessage = new CloudQueueMessage(newsMessage);
            await queue.AddMessageAsync(articleMessage);
        }

        static async Task<string> ReceiveArticleAsync()
        {
            CloudQueue queue = GetQueue();
            bool exists = await queue.ExistsAsync();
            if (exists)
            {
                CloudQueueMessage retrievedArticle = await queue.GetMessageAsync();
                if (retrievedArticle != null)
                {
                    string newsMessage = retrievedArticle.AsString;
                    await queue.DeleteMessageAsync(retrievedArticle);
                    return newsMessage;
                }
            }

            return "<queue empty or not created>";
        }

        static CloudQueue GetQueue()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageQueueConnection);

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            return queueClient.GetQueueReference("newsqueue");
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