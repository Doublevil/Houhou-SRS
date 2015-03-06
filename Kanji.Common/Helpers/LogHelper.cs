using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace Kanji.Common.Helpers
{
    public static class LogHelper
    {
        public static string GetLogFilePath()
        {
            return ((log4net.Appender.RollingFileAppender)log4net.LogManager.GetRepository().GetAppenders()[0]).File;
        }

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
