using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.Helpers
{
    static class ResourceHelper
    {
        #region Constants

        private static readonly string ResourceRootPath = "Kanji.Interface.Data.Resources.";

        public static readonly string HomePageFutureChanges = "HomePageFutureChanges.xml";
        public static readonly string HomePageChangelog = "HomePageChangelog.xml";
        public static readonly string HomePageHelp = "HomePageHelp.xml";

        public static readonly string TrayIconIdle = "TrayIconIdle.ico";
        public static readonly string TrayIconAlert = "TrayIconAlert.ico";

        #endregion

        #region Methods

        public static Stream GetResourceStream(string resourcePath)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceRootPath + resourcePath);
        }

        #endregion
    }
}
