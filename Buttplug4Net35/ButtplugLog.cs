using System;
using JetBrains.Annotations;

namespace Buttplug4Net35
{
    internal class ButtplugLog : IButtplugLog
    {
        //[NotNull]
        //private readonly ILog _log;

        public event EventHandler<ButtplugLogMessageEventArgs> LogMessageReceived;

        public ButtplugLog()
        {
        }

        public void Trace(string aMsg, bool aLocalOnly)
        {
            Console.WriteLine($"[BP-Trace] {aMsg}");
            // _log.Trace(aMsg);
            if (!aLocalOnly)
            {
                LogMessageReceived?.Invoke(this, new ButtplugLogMessageEventArgs(ButtplugLogLevel.Trace, aMsg));
            }
        }

        public void Debug(string aMsg, bool aLocalOnly)
        {
            Console.WriteLine($"[BP-Debug] {aMsg}");
            //_log.Debug(aMsg);
            if (!aLocalOnly)
            {
                LogMessageReceived?.Invoke(this, new ButtplugLogMessageEventArgs(ButtplugLogLevel.Debug, aMsg));
            }
        }

        public void Info(string aMsg, bool aLocalOnly)
        {
            Console.WriteLine($"[BP-Info] {aMsg}");
            //_log.Info(aMsg);
            if (!aLocalOnly)
            {
                LogMessageReceived?.Invoke(this, new ButtplugLogMessageEventArgs(ButtplugLogLevel.Info, aMsg));
            }
        }

        public void Warn(string aMsg, bool aLocalOnly)
        {
            Console.WriteLine($"[BP-Warn] {aMsg}");
            //_log.Warn(aMsg);
            if (!aLocalOnly)
            {
                LogMessageReceived?.Invoke(this, new ButtplugLogMessageEventArgs(ButtplugLogLevel.Warn, aMsg));
            }
        }

        public void Error(string aMsg, bool aLocalOnly)
        {
            Console.WriteLine($"[BP-Error] {aMsg}");
            // _log.Error(aMsg);
            if (!aLocalOnly)
            {
                LogMessageReceived?.Invoke(this, new ButtplugLogMessageEventArgs(ButtplugLogLevel.Error, aMsg));
            }
        }

        public void Fatal(string aMsg, bool aLocalOnly)
        {
            Console.WriteLine($"[BP-Fatal] {aMsg}");
            //_log.Fatal(aMsg);
            if (!aLocalOnly)
            {
                LogMessageReceived?.Invoke(this, new ButtplugLogMessageEventArgs(ButtplugLogLevel.Fatal, aMsg));
            }
        }

        public event EventHandler<LogExceptionEventArgs> OnLogException;

        public void LogException(Exception aEx, bool aLocalOnly = true, string aMsg = null)
        {
            Error((aEx?.GetType().ToString() ?? "Unknown Exception") + ": " +
                  (aMsg ?? (aEx != null ? (aEx.Message + "\n" + aEx.StackTrace) : "Unknown Exception")), aLocalOnly);
            OnLogException?.Invoke(this, new LogExceptionEventArgs(aEx, aLocalOnly, aMsg));
        }
    }
}
