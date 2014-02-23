using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji.Common.Helpers;
using Kanji.Common.Models;
using Kanji.Interface.Helpers;
using Kanji.Interface.Models;
using Kanji.Interface.Utilities;

namespace Kanji.Interface.Actors
{
    class PipeActor
    {
        #region Static

        /// <summary>
        /// Creates and initializes the instance.
        /// </summary>
        public static void Initialize(NamedPipeHandler handler)
        {
            Instance = new PipeActor(handler);
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static PipeActor Instance { get; private set; }

        #endregion

        #region Fields

        private NamedPipeHandler _handler;

        #endregion

        #region Constructor

        private PipeActor(NamedPipeHandler handler)
        {
            _handler = handler;
            _handler.MessageReceived += OnMessageReceived;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Processes the given message to a command, and executes the command.
        /// </summary>
        /// <param name="message">Message to process.</param>
        private void ProcessMessage(string message)
        {
            PipeMessageEnum? command = ParsingHelper.ParseEnum<PipeMessageEnum>(message);
            if (command.HasValue)
            {
                // Log the pipe command received.
                LogHelper.GetLogger(this.GetType().Name).WarnFormat(
                        "Received \"{0}\" pipe command.", command.Value);

                // Shutdown command.
                if (command.Value == PipeMessageEnum.OpenOrFocus)
                {
                    NavigationActor.Instance.OpenOrFocus();
                }
                else
                {
                    // Unhandled command. Ignore (but log).
                    LogHelper.GetLogger(this.GetType().Name).WarnFormat(
                        "Unhandled pipe command: \"{0}\". Ignoring.", command.Value);
                }
            }
        }

        #region Event callbacks

        /// <summary>
        /// Event callback.
        /// Called when a message is received by the associated pipe handler.
        /// </summary>
        private void OnMessageReceived(object sender, NamedPipeMessageEventArgs e)
        {
            ProcessMessage(e.Message);
        }

        #endregion

        #endregion
    }
}
