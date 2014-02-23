using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji.Common.Helpers;

namespace Kanji.Interface.Helpers
{
    static class DispatcherHelper
    {
        private static readonly string LogName = "DispatcherHelper";
        private static readonly string LogEntryFormat =
            "Could not execute an action on the dispatcher:\n{0}";

        /// <summary>
        /// Safely invokes the given synchronous action on the dispatcher.
        /// </summary>
        /// <param name="action">Action to invoke.</param>
        public static void Invoke(Action action)
        {
            
                App.Current.Dispatcher.Invoke(action);
            
        }

        /// <summary>
        /// Safely invokes the given synchronous action on the dispatcher.
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="action">Function returning a value of type T.</param>
        /// <returns>The value returned by the function,
        /// or the default value of T if the call fails.</returns>
        public static T Invoke<T>(Func<T> action)
        {
            try
            {
                return App.Current.Dispatcher.Invoke<T>(action);
            }
            catch (Exception ex)
            {
                LogHelper.GetLogger(LogName).WarnFormat(LogEntryFormat, ex);
                return default(T);
            }
        }

        /// <summary>
        /// Safely invokes the given asynchronous action on the dispatcher.
        /// </summary>
        /// <param name="action">Action to invoke.</param>
        public static void InvokeAsync(Action action)
        {
            try
            {
                App.Current.Dispatcher.InvokeAsync(action);
            }
            catch (Exception ex)
            {
                LogHelper.GetLogger(LogName).WarnFormat(LogEntryFormat, ex);
            }
        }
    }
}
