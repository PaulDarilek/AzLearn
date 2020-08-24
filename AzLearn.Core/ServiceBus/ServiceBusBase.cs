using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Hosting;

namespace AzLearn.Core.ServiceBus
{
    public class ServiceBusBase : BackgroundService
    {
        public bool IsExecuting { get; private set; }
        public Action<string> TraceAction { get; set; }
        public Func<Message, Task> ProcessFunction { get; set; }
        public ServiceBusSettings Settings { get; }
        private QueueClient Client { get; }

        public ServiceBusBase(ServiceBusSettings settings, Func<Message, Task> processFunction)
        {
            Settings = settings;
            ProcessFunction = processFunction ?? throw new ArgumentNullException(nameof(processFunction));
            TraceAction = Console.Out.WriteLine;
            Client = new QueueClient(Settings.ServiceBusConnectionString, Settings.ServiceBusQueueName);
        }

        public void Stop() => IsExecuting = false;
        public async Task SendAync(Message message) => await Client.SendAsync(message);
        
        public async Task SendAync(IEnumerable<Message> messageList) => await Client.SendAsync(messageList?.ToList());

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() => IsExecuting = false);

            // Configure the MessageHandler Options in terms of exception handling, number of concurrent messages to deliver etc.
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionHandlerAsync)
            {
                // Maximum number of Concurrent calls to the callback `ProcessMessagesAsync`, set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = Settings.ServiceBusConcurrency ?? 1,

                // Indicates whether MessagePump should automatically complete the messages after returning from User Callback.
                // False below indicates the Complete will be handled by the User Callback as in `ProcessMessagesAsync` below.
                AutoComplete = false,

                // Experiment with a given timeframe to allow use in azure functions.
                MaxAutoRenewDuration = TimeSpan.FromSeconds(Settings.ServiceBusAutoRenewSeconds ?? 90),
            };

            // Start a message pump.
            Client.RegisterMessageHandler(HandleMessageAsync, messageHandlerOptions);

            // Wait until CancellationToken is signaled, so that message pump will process messages.
            IsExecuting = true;
            while (IsExecuting == true)
            {
                Trace.WriteLine($"Message pump running @ {DateTime.Now:HH:mm:ss}");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task HandleMessageAsync(Message message, CancellationToken stoppingToken)
        {
            IsExecuting &= !stoppingToken.IsCancellationRequested;

            if (IsExecuting && message != null)
            {
                // Process the message
                await ProcessFunction(message);

                // Complete the message so that it is not received again.
                // This can be done only if the queueClient is created in ReceiveMode.PeekLock mode (which is default).
                await Client.CompleteAsync(message.SystemProperties.LockToken);

                // Note: Use the cancellationToken passed as necessary to determine if the queueClient has already been closed.
                // If queueClient has already been Closed, you may chose to not call CompleteAsync() or AbandonAsync() etc. calls 
                // to avoid unnecessary exceptions.
            }
        }

        private async Task ExceptionHandlerAsync(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            var text = new StringBuilder();
            try
            {
                text.AppendLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
                var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
                text.AppendLine("Exception context for troubleshooting:");
                text.AppendLine($"- Endpoint: {context.Endpoint}");
                text.AppendLine($"- Entity Path: {context.EntityPath}");
                text.AppendLine($"- Executing Action: {context.Action}");

            }
            finally
            {
                TraceAction?.Invoke(text.ToString());
            }
            await Task.CompletedTask;
        }

        public override void Dispose()
        {
            IsExecuting = false;    
            if (! Client.IsClosedOrClosing)
            {
                Client.CloseAsync().Wait();
            }
            base.Dispose();
        }
    }
}
