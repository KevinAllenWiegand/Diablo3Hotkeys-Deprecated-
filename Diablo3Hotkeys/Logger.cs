using System;
using System.Diagnostics;
using System.IO;

namespace DiabloIIIHotkeys
{
    internal class Logger
    {
        private static Lazy<Logger> _Instance = new Lazy<Logger>(() => new Logger());

        private const string _LogFileName = "Log.txt";

        private bool _WriteLogToFile = false;
        public bool WriteLogToFile
        {
            set { _WriteLogToFile = value; }
        }

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

            if (_WriteLogToFile)
            {
                var timestamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                File.AppendAllText(_LogFileName, $"[{timestamp}] {message}{Environment.NewLine}");
            }

            OnLogMessage(message);
        }

        private void OnLogMessage(string message)
        {
            var handler = LogMessage;

            handler?.Invoke(this, new LogMessageEventArgs(message));
        }
    }
}
