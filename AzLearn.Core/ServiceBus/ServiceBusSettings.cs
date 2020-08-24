
namespace AzLearn.Core.ServiceBus
{
    public class ServiceBusSettings
    {
        public string ServiceBusConnectionString { get; set; }
        public string ServiceBusQueueName { get; set; }
        public int? ServiceBusConcurrency { get; set; }
        public int? ServiceBusAutoRenewSeconds { get; set; }
    }
}
