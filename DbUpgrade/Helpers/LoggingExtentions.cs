using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DbUpgrade.Helpers
{
    public static  class LoggingExtentions
    {
        public static void Info(this ILogger logger, string message, params object[] args)
        {
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            logger.LogInformation(message, args);
            Console.ForegroundColor = currentColor;
        }

        public static void Error(this ILogger logger, string message, params object[] args)
        {
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            logger.LogError(message, args);
            Console.ForegroundColor = currentColor;
        }

        public static void Error(this ILogger logger, Exception ex, string message, params object[] args)
        {
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            logger.LogError(ex, message, args);
            Console.ForegroundColor = currentColor;
        }

        public static void Debug(this ILogger logger, string message, params object[] args)
        {
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            logger.LogInformation(message, args);
            Console.ForegroundColor = currentColor;
        }
    }
}
