using System;

namespace DiabloIIIHotkeys
{
    internal class ProfileRunningStateEventArgs : EventArgs
    {
        public bool IsRunning { get; }

        public ProfileRunningStateEventArgs(bool isRunning)
        {
            IsRunning = isRunning;
        }
    }
}
