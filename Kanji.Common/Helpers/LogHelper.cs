using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace Kanji.Common.Helpers
{
    public static class LogHelper
    {
        public static void InitializeLoggingSystem()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        public static ILog GetLogger(string name)
        {
            return LogManager.GetLogger(name);
        }
    }
}
