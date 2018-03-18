using log4net;

namespace WebApiInsight.Agent.Util
{
    public static class LogHelper
    {
        public static ILog GetLogger()
        {
            return LogManager.GetLogger("WebApiInsight.Agent");
        }
    }
}