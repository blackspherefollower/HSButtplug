﻿using System;
using Buttplug4Net35.Messages;
using JetBrains.Annotations;

namespace Buttplug4Net35
{
    /// <summary>
    /// Event wrapper for log message events.
    /// </summary>
    public class ButtplugLogMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Log message.
        /// </summary>
        [NotNull]
        public Log LogMessage { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtplugLogMessageEventArgs"/> class.
        /// </summary>
        /// <param name="aLogLevel">Log level</param>
        /// <param name="aMessage">Log message</param>
        public ButtplugLogMessageEventArgs(ButtplugLogLevel aLogLevel, string aMessage)
        {
            LogMessage = new Log(aLogLevel, aMessage);
        }
    }
}
