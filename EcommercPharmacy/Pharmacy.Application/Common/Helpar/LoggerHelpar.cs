using Microsoft.Extensions.Logging;

namespace Pharmacy.Application.Common.Helpar;

public static class LoggerHelpar
{
    public static void LogSection(this ILogger logger, string title, string message, LogLevel level = LogLevel.Information)
    {
        var color = Console.ForegroundColor;

        Console.ForegroundColor = level switch
        {
            LogLevel.Error => ConsoleColor.Red,
            LogLevel.Warning => ConsoleColor.Yellow,
            LogLevel.Information => ConsoleColor.Green,
            LogLevel.Debug => ConsoleColor.Cyan,
            _ => ConsoleColor.White
        };

        Console.WriteLine();
        Console.WriteLine("╔══════════════════════════════════════════════════════╗");
        Console.WriteLine($"║ {DateTime.Now:HH:mm:ss} | {title.ToUpper(),-40} ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════╝");

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(message);
        Console.WriteLine();

        logger.Log(level, $"{title}: {message}");

        Console.ForegroundColor = color;
    }
}

