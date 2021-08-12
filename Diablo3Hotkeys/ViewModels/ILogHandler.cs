namespace DiabloIIIHotkeys.ViewModels
{
    internal interface ILogHandler
    {
        public void CopyLogToClipboard();
        public void ClearLog();
        public void ToggleAutoScrollState();
    }
}
