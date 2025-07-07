using Serilog;

namespace Helpers
{
    public static class Logger
    {
        public static void Init()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("Logs/testlog-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}