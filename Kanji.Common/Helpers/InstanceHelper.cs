using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kanji.Common.Helpers
{
    public static class InstanceHelper
    {
        #if DEBUG

        // Application GUID will provide us a unique string to share the mutex.
        public static readonly string InterfaceApplicationGuid =
            "{22749F1E-0074-4B45-A577-A42199FEE100}";

        #else

        // Application GUID will provide us a unique string to share the mutex.
        public static readonly string InterfaceApplicationGuid =
            "{E9216009-745B-4C3B-B270-172197A859DD}";

        #endif
    }
}
