namespace DiabloIIIHotkeys.Commands
{
    internal class ProfileKeyActionParameters
    {
        public static readonly ProfileKeyActionParameters StopActionParameters = new ProfileKeyActionParameters(ProfileKeyAction.Stop);
        public static readonly ProfileKeyActionParameters ResetActionParameters = new ProfileKeyActionParameters(ProfileKeyAction.Reset);
        public static readonly ProfileKeyActionParameters ToggleRunningMacroProfileParameters = new ProfileKeyActionParameters(ProfileKeyAction.ToggleProfileRunning);
        public static readonly ProfileKeyActionParameters SendRightMouseDownParameters = new ProfileKeyActionParameters(ProfileKeyAction.RightMouseDown);

        public ProfileKeyAction Action { get; }
        public string Parameter { get; }

        public ProfileKeyActionParameters(ProfileKeyAction action, string parameter)
        {
            Action = action;
            Parameter = parameter;
        }

        private ProfileKeyActionParameters(ProfileKeyAction action)
            : this(action, null)
        {
        }
    }
}
