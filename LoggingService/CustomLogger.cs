using Contracts;
using Serilog;

namespace LoggingService
{
    public class CustomLogger(ILogger logger) : ICustomLogger
    {
        public void LogDebug(string message) => logger.Debug(message);
        public void LogInformation(string message) => logger.Information(message);
        public void LogWarning(string message) => logger.Warning(message);
        public void LogError(string message) => logger.Error(message);
    }
}
