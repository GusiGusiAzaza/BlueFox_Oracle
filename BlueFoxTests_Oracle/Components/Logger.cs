using log4net;
using log4net.Config;

namespace BlueFoxTests_Oracle.Components
{
    public static class Logger
    {
        public static readonly ILog Log = LogManager.GetLogger("LOGGER");

        static Logger()
        {
            XmlConfigurator.Configure();
        }
    }
}
