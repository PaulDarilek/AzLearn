using AzLearn.Core.ServiceBus;
using FluentAssertions;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AzLearn.Core.Tests
{
    public class ServiceBusTests
    {
        private readonly ConcurrentQueue<Message> _messages = new ConcurrentQueue<Message>();
        private CancellationTokenSource CancelSource { get; set; }
        private CancellationToken CancelToken => CancelSource.Token;

        [Fact]
        public void ServiceBusShouldBeConfigured()
        {
            var service = Create();
            Assert.NotNull(service);
            Assert.NotNull(service.Settings);
            Assert.NotNull(service.Settings.ServiceBusConnectionString);
            Assert.NotNull(service.Settings.ServiceBusQueueName);
            Assert.NotNull(service.Settings.ServiceBusConcurrency);
            Assert.NotNull(service.Settings.ServiceBusAutoRenewSeconds);
        }

        [Fact]
        public async Task ServiceBusBase_SendAync_1()
        {
            // Arrange
            int expectedCount = 1;
            using var service = Create();
            var message = CreateMessages(expectedCount).First();

            // Act
            await service.SendAync(message);

            // Assert
            await WaitForExpectedMessagesAsync(expectedCount);
            _messages.Count.Should().BeGreaterOrEqualTo(expectedCount);
        }

        [Fact]
        public async Task ServiceBusBase_SendAync_3()
        {
            // Arrange
            int expectedCount = 3;
            using var service = Create();
            var messages = CreateMessages(expectedCount);

            // Act
            await service.SendAync(messages);

            // Assert
            await WaitForExpectedMessagesAsync(expectedCount);
            _messages.Count.Should().BeGreaterOrEqualTo(expectedCount);
        }

        [Fact]
        public async Task ServiceBusBase_StartAsync()
        {
            // Arrange
            int expectedCount = 8;
            using var service = Create(maxTestTimeSeconds: 7 + expectedCount);
            var messages = CreateMessages(expectedCount);
            await service.SendAync(messages);
            service.IsExecuting.Should().BeTrue();
            await WaitForExpectedMessagesAsync(expectedCount);
            // Doesn't matter if any messages were returned.
        }


        private ServiceBusBase Create(int maxTestTimeSeconds = 360)
        {
            _messages.Clear();
            int minutes = maxTestTimeSeconds / 60;
            int seconds = maxTestTimeSeconds % 60;
            CancelSource = new CancellationTokenSource(new TimeSpan(0, minutes, seconds));

            var settings = ConfigurationService.GetConfigurationSettings<ServiceBusSettings>();
            var service = new ServiceBusBase(settings, AddMessageAsync);
            service.StartAsync(CancelToken);
            return service;
        }

        private IEnumerable<Message> CreateMessages(int count, [CallerMemberName] string callerMemberName = null)
        {
            for (int i = 1; i <= count; i++)
            {
                string messageBody = $"Message_{callerMemberName}_{i}";
                var message = new Message(Encoding.UTF8.GetBytes(messageBody)) { MessageId = messageBody };
                yield return message;
            }
        }

        private async Task WaitForExpectedMessagesAsync(int expectedCount)
        {
            while(_messages.Count < expectedCount && !CancelToken.IsCancellationRequested)
            {
                try { await Task.Delay(100, CancelToken); }
                catch (TaskCanceledException) { }
            }
            CancelSource.Cancel();
        }
        private async Task AddMessageAsync(Message  message)
        {
            _messages.Enqueue(message);
            await Task.CompletedTask;
        }
    }
}
