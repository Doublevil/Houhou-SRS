using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Kanji.Common.Helpers;
using Kanji.Common.Models;
using Kanji.Interface.Actors;
using Kanji.Interface.Business;
using Kanji.Interface.Helpers;
using Kanji.Interface.Models;
using Kanji.Interface.Utilities;
using System.Diagnostics;

namespace Kanji.Interface
{
    class Program
    {
        #region Static fields

        // Mutex used to make sure only one instance of the application is running.
        private static Mutex RunOnceMutex = new Mutex(true, InstanceHelper.InterfaceApplicationGuid);

        public static bool RunMainWindow;

        #endregion

        [STAThread]
        public static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location));

            // This app uses a mutex to make sure it is started only one time.
            // In this context, keep in mind that the mutex is the only
            // shared resource between instances of the application.

            // Acquire parameters.
            // There is one optional parameter:
            // 
            // [0] - Boolean - Indicates if the Main Window should be started.
            // - Default is True.

            RunMainWindow = (args.Any() ? ParsingHelper.ParseBool(args[0]) : null) ?? true;

            // Check the mutex.
            if (RunOnceMutex.WaitOne(TimeSpan.Zero, true))
            {
                // The application is not already running. So let's start it.
                Run();

                // Once the application is shutdown, release the mutex.
                RunOnceMutex.ReleaseMutex();
            }
            else
            {
                // The application is already running.
                // Transmit a command to open/focus the main window using the pipe system.
                using (NamedPipeHandler handler = new NamedPipeHandler(InstanceHelper.InterfaceApplicationGuid))
                {
                    handler.Write(PipeMessageEnum.OpenOrFocus.ToString());
                }
            }
        }

        /// <summary>
        /// Initializes the application, runs it, and manages
        /// the resources.
        /// </summary>
        public static void Run()
        {
            // Initialize the logging system.
            LogHelper.InitializeLoggingSystem();

            // Initialize the configuration system.
            ConfigurationHelper.InitializeConfiguration();

            // Create the application.
            App app = new App();

            // Load the navigation actor.
            NavigationActor.Instance = new NavigationActor();

            // Start loading user resources.
            RadicalStore.Instance.InitializeAsync();
            SrsLevelStore.Instance.InitializeAsync();

            // Load the autostart configuration.
            AutostartBusiness.Instance.Load();

            // Start the version business.
            VersionBusiness.Initialize();

            // Start the SRS business.
            SrsBusiness.Initialize();

            using (NamedPipeHandler pipeHandler = new NamedPipeHandler(InstanceHelper.InterfaceApplicationGuid))
            {
                // Listen for incoming pipe messages, to allow other processes to
                // communicate with this one.
                PipeActor.Initialize(pipeHandler);
                pipeHandler.StartListening();

                // Run the app.
                app.InitializeComponent();
                AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
                app.DispatcherUnhandledException += OnUnhandledException;
                TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
                app.Run();

                // The execution blocks here until the application exits.
            }
        }

        static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            OnUnhandledException(e.Exception);
        }

        /// <summary>
        /// Event trigger. Called when an unhandled exception is thrown by any thread.
        /// </summary>
        static void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            OnUnhandledException((e.ExceptionObject as Exception) ?? new Exception("Unknown fatal error."));
        }

        /// <summary>
        /// Event trigger. Called when an unhandled exception is thrown by the Dispatcher thread.
        /// </summary>
        private static void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            OnUnhandledException(e.Exception);
        }

        /// <summary>
        /// Logs an unhandled exception and terminates the application properly.
        /// </summary>
        /// <param name="ex">Unhandled exception.</param>
        private static void OnUnhandledException(Exception ex)
        {
            LogHelper.GetLogger("Main").Fatal("A fatal exception occured:", ex);

            #if DEBUG
            throw ex;
            #else

            DispatcherHelper.Invoke(() =>
            {
                if (System.Windows.MessageBox.Show(
                    string.Format("It appears that Houhou has been vanquished by an evil {0}.{1}"
                    + "Houhou will now shutdown. Sorry for the inconvenience.{1}"
                    + "Houhou's last words were: \"{2}\". Oh the pain it must have been.{1}{1}"
                    + "Please send me a mail report along with your log file if you think I should fix the issue.{1}"
                    + "Do you want to open the log?",
                        ex.GetType().Name,
                        Environment.NewLine,
                        ex.Message),
                    "Fatal error",
                    System.Windows.MessageBoxButton.YesNoCancel,
                    System.Windows.MessageBoxImage.Error) == System.Windows.MessageBoxResult.Yes)
                {
                    try
                    {
                        Process.Start(LogHelper.GetLogFilePath());
                    }
                    catch (Exception ex2)
                    {
                        LogHelper.GetLogger("Main").Fatal("Failed to open the log after fatal exception. Double fatal shock.", ex2);

                        System.Windows.MessageBox.Show(
                            string.Format("Okay, so... the log file failed to open.{0}"
                            + "Um... I'm sorry. Now that's embarrassing...{0}"
                            + "Hey, listen, the log file should be there:{0}"
                            + "\"C:/Users/<YourUserName>/AppData/Local/Houhou SRS/Logs\"{0}"
                            + "If you still cannot get it, well just contact me without a log.{0}",
                                Environment.NewLine),
                            "Double fatal error shock",
                            System.Windows.MessageBoxButton.OK,
                            System.Windows.MessageBoxImage.Error);
                    }
                }
            });

            Environment.Exit(1);
            #endif
        }

        /// <summary>
        /// Causes the application to shutdown.
        /// </summary>
        public static void Shutdown()
        {
            Kanji.Interface.Properties.Settings.Default.Save();
            DispatcherHelper.Invoke(() => { App.Current.Shutdown(); });
        }
    }
}
