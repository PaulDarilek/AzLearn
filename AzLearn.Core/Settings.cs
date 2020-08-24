namespace AzLearn.Core
{
    public class Settings
    {
        public string StorageQueueConnection { get; set; }
        public string StorageContainerName { get; set; }
        public string StorageAccountName { get; set; }
        public string StorageAccountKey { get; set; }
        public string EventHubName { get; set; }
        public string EventHubConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public string QueueName { get; set; }
    }
}
