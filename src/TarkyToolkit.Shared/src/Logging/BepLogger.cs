using BepInEx.Logging;

namespace TarkyToolkit.Shared.Logging
{
    public class BepLogger : Logger
    {
        private readonly ManualLogSource _logger;

        public BepLogger(ManualLogSource logger)
        {
            _logger = logger;
        }

        public override void LogInfo(string message)
        {
            _logger.LogInfo(message);
        }

        public override void LogDebug(string message)
        {
            _logger.LogDebug(message);
        }

        public override void LogWarning(string message)
        {
            _logger.LogWarning(message);
        }

        public override void LogError(string message)
        {
            _logger.LogError(message);
        }
    }
}
