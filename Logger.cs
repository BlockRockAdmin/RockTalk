using Microsoft.Extensions.Logging;

namespace RockTalk
{
    public class Logger
    {
        private readonly ILogger _logger;

        public Logger(ILoggerFactory factory)
        {
            _logger = factory.CreateLogger(nameof(Logger));
        }

        public void LogInformation(string message, params object[] args)
        {
            _logger.LogInformation(message, args);
        }

        public void LogDebug(string message, params object[] args)
        {
            _logger.LogDebug(message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            _logger.LogWarning(message, args);
        }

        public void LogError(Exception ex, string message, params object[] args)
        {
            _logger.LogError(ex, message, args);
        }
    }
}
