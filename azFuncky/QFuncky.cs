using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace azFuncky
{
    public static class QFuncky
    {
        [FunctionName("QFuncky")]
        public static void Run([QueueTrigger("myqueue-items", Connection = "todo")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
