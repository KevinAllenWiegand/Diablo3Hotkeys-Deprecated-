using System;
using System.Diagnostics;

namespace DiabloIIIHotkeys
{
    internal class Logger
    {
        private static Lazy<Logger> _Instance = new Lazy<Logger>(() => new Logger());

        public static Logger Instance
        {
            get { return _Instance.Value; }
        }

        public event EventHandler<LogMessageEventArgs> LogMessage;

        private Logger()
        {
        }

        public void Log(string message)
        {
            Trace.WriteLine(message);
            OnLogMessage(message);
        }

        private void OnLogMessage(string message)
        {
            var handler = LogMessage;

            handler?.Invoke(this, new LogMessageEventArgs(message));
        }
    }
}
