using log4net;

namespace CustomsForgeNotifier
{
    public static class LogHelper
    {
        private static readonly ILog Logger = LogManager.GetLogger("CustomsForgeNotifier");

        public static ILog GetLog()
        {
            return Logger;
        }
    }
}