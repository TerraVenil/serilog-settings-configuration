using System;

using Microsoft.Extensions.Configuration;
using Serilog;
using System.IO;
using System.Linq;
using Serilog.Core;
using Serilog.Events;
using System.Collections.Generic;

namespace Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            do
            {
                logger.ForContext<Program>().Information("Hello, world!");
                logger.ForContext<Program>().Error("Hello, world!");
            }
            while(!args.Contains("--run-once") && (Console.ReadKey().KeyChar != 'q'));
        }
    }

    // The filter syntax in the sample configuration file is
    // processed by the Serilog.Filters.Expressions package.
    public class CustomFilter : ILogEventFilter
    {
        public bool IsEnabled(LogEvent logEvent)
        {
            return true;
        }
    }

    public class LoginData
    {
        public string Username;
        public string Password;
    }

    public class CustomPolicy : IDestructuringPolicy
    {
        public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue result)
        {
            result = null;

            if(value is LoginData)
            {
                result = new StructureValue(
                    new List<LogEventProperty>
                    {
                        new LogEventProperty("Username", new ScalarValue(((LoginData)value).Username))
                    });
            }

            return (result != null);
        }
    }
}
