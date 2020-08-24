using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace azWebJob1
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new HostBuilder();
            //builder.ConfigureAppConfiguration(config => { 
            //    config.Sources.Add()
            //});

            builder.ConfigureWebJobs(b =>
            {
                b.AddAzureStorageCoreServices();
                b.AddAzureStorage();
            });

            builder.ConfigureLogging((context, b) =>
            {
                b.AddConsole();
            });

            using (IHost host = builder.Build())
            {
                host.Run();
            }
        }
    }
}
