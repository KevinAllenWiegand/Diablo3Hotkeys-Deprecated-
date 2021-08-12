using System;

namespace DiabloIIIHotkeys
{
    internal class LogMessageEventArgs : EventArgs
    {
        public string Message { get; }

        public LogMessageEventArgs(string message)
        {
            Message = message;
        }

        public override string ToString()
        {
            return Message;
        }
    }
}
