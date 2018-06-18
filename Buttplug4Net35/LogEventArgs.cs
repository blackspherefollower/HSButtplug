using System;
using Buttplug4Net35.Messages;
using JetBrains.Annotations;

namespace Buttplug4Net35
{
    /// <summary>
    /// Event wrapper for a Buttplug Log message. Used when the server is sending log entries to the client.
    /// </summary>
    public class LogEventArgs : EventArgs
    {
        /// <summary>
        /// The Buttplug Log message.
        /// </summary>
        [NotNull]
        public readonly Log Message;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEventArgs"/> class.
        /// </summary>
        /// <param name="aMsg">A Buttplug Log message.</param>
        public LogEventArgs(Log aMsg)
        {
            Message = aMsg;
        }
    }
}