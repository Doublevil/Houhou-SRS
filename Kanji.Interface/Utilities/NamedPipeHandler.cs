using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji.Interface.Models;

namespace Kanji.Interface.Utilities
{
    /// <summary>
    /// Makes uses of named pipes to allow inter-process communication.
    /// </summary>
    class NamedPipeHandler : IDisposable
    {
        #region Constants

        // Defines the message used to stop listening.
        private static readonly string StopMessage = "{CB00C58F-D5AE-411D-A451-DDFC1F5B38F5}";

        // Defines the timeout in ms for a pipe writing operation.
        private static readonly int WriteTimeout = 1000;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the pipe name.
        /// </summary>
        /// <remarks>If set while listening, the new value will not be applied
        /// until the next message is received.</remarks>
        public string PipeName { get; set; }

        /// <summary>
        /// Gets a value indicating if the actor is listening for pipe messages.
        /// </summary>
        public bool IsListening { get; private set; }

        #endregion

        #region Events

        public delegate void MessageReceivedHandler(object sender, NamedPipeMessageEventArgs e);
        /// <summary>
        /// Triggered when a message is received by the listening thread.
        /// </summary>
        public event MessageReceivedHandler MessageReceived;

        #endregion

        #region Constructors

        /// <summary>
        /// Builds a named pipe handler with the given pipe name.
        /// </summary>
        /// <param name="pipeName">Name of the pipe to handle.</param>
        public NamedPipeHandler(string pipeName)
        {
            PipeName = pipeName;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Starts the server thread listening for incoming messages.
        /// </summary>
        /// <remarks>StopListening or Dispose MUST be called in order
        /// for the application to exit properly. Otherwise the
        /// thread will continue listening forever.</remarks>
        public void StartListening()
        {
            Task.Run(() => { Listen(); });
        }

        /// <summary>
        /// Listens for incoming messages until the StopMessage
        /// is received.
        /// </summary>
        private void Listen()
        {
            IsListening = true;

            while (true)
            {
                using (NamedPipeServerStream server = new NamedPipeServerStream(PipeName))
                {
                    server.WaitForConnection();
                    using (var reader = new BinaryReader(server))
                    {
                        // Acquire the string message.
                        string message = reader.ReadString();

                        // Test if the message matches the stop message.
                        if (message == StopMessage)
                        {
                            // If it does, break from the infinite loop.
                            break;
                        }
                        // Otherwise, trigger the message event.
                        else if (MessageReceived != null)
                        {
                            MessageReceived(this,
                                new NamedPipeMessageEventArgs(message));
                        }
                    }
                }
            }

            IsListening = false;
        }

        /// <summary>
        /// Attempts to write the given message to the pipe.
        /// </summary>
        /// <param name="message">Message to write.</param>
        /// <returns>True if the message was successfuly written.
        /// False in case of timeout or any other failure.</returns>
        public bool Write(string message)
        {
            try
            {
                NamedPipeClientStream client = new NamedPipeClientStream(PipeName);
                client.Connect(WriteTimeout);
                using (var writer = new BinaryWriter(client))
                {
                    writer.Write(message);
                }
            }
            catch (TimeoutException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Attempts to stops the listening thread.
        /// </summary>
        public void StopListening()
        {
            Write(StopMessage);
        }

        /// <summary>
        /// Stops listening if needed.
        /// </summary>
        public void Dispose()
        {
            if (IsListening)
            {
                StopListening();
            }
        }

        #endregion
    }
}
