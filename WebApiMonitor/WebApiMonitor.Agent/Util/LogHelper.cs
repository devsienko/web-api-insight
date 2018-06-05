using log4net;

namespace WebApiMonitor.Agent.Util
{
    public static class LogHelper
    {
        public static ILog GetLogger()
        {
            return LogManager.GetLogger("WebApiMonitor.Agent");
        }
    }
}