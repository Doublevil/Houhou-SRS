using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Kanji.Common.Helpers;
using Microsoft.Win32;

namespace Kanji.Interface.Business
{
    public class AutostartBusiness
    {
        #region Constants

        private static readonly string RegistryValueName = "Houhou SRS Tray";
        private static readonly string RegistryKeyPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

        #endregion

        #region Singleton

        private static AutostartBusiness _instance;

        /// <summary>
        /// Gets the registered instance.
        /// </summary>
        public static AutostartBusiness Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AutostartBusiness();
                }

                return _instance;
            }
        }

        #endregion

        #region Fields

        private bool _savedAutoStart;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a boolean defining if the application is launched at startup.
        /// </summary>
        public bool AutoStart { get; set; }

        #endregion

        #region Constructor

        private AutostartBusiness()
        {

        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads the state.
        /// </summary>
        public void Load()
        {
            // Load the auto-start status.
            AutoStart = GetAutoStartStatus();
            _savedAutoStart = GetAutoStartStatus();
        }

        /// <summary>
        /// Gets and returns the auto start status.
        /// </summary>
        private bool GetAutoStartStatus()
        {
            bool result = false;

            try
            {
                using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, true))
                {
                    result = regKey.GetValue(RegistryValueName) != null;
                    regKey.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.GetLogger(GetType().Name).Warn(
                    "Could not load startup configuration.", ex);
            }

            return result;
        }

        /// <summary>
        /// Saves the status.
        /// </summary>
        public void Save()
        {
            if (AutoStart != _savedAutoStart)
            {
                _savedAutoStart = AutoStart;
                SaveAutoStart();
            }
        }

        /// <summary>
        /// Saves the auto start status.
        /// </summary>
        private void SaveAutoStart()
        {
            try
            {
                using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, true))
                {
                    if (AutoStart)
                    {
                        // Set the value.
                        regKey.SetValue(RegistryValueName, string.Format("\"{0}\" False",
                            System.Reflection.Assembly.GetEntryAssembly().Location));
                    }
                    else
                    {
                        // Delete the value.
                        regKey.DeleteValue(RegistryValueName, false);
                    }

                    regKey.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.GetLogger(GetType().Name).Warn(
                    "Could not save startup configuration.", ex);
            }
        }

        #endregion
    }
}
