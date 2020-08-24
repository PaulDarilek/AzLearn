using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;

namespace AzLearn.Core
{
    [DebuggerStepThrough]
    public static class ConfigurationService
    {
        public static T GetConfigurationSettings<T>(string[] args = null) where T: class
            => GetConfiguration<T>(args).Get<T>();

        public static IConfigurationRoot GetConfiguration<T>(string[] args) where T : class
        {
            string env =
                Environment.GetEnvironmentVariable("DOTNET_Environment") ??
                Environment.GetEnvironmentVariable("Environment") ??
                string.Empty;

            var builder =
                new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile($"appsettings{env}.json", true)
                .AddEnvironmentVariables()
                ;

            if (args != null)
            {
                builder.AddCommandLine(args);
            }

            if (env.Equals("Development", StringComparison.OrdinalIgnoreCase) ||
                env.Equals("Dev", StringComparison.OrdinalIgnoreCase) ||
                Environment.UserInteractive)
            {
                builder.AddUserSecrets<T>();
            }

            var host = builder.Build();
            return host;
        }
    }
}
