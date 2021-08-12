using DiabloIIIHotkeys.Commands;

namespace DiabloIIIHotkeys
{
    internal class HotkeyDefinition
    {
        public uint HotkeyModifiers { get; }
        public uint HotkeyKey { get; }
        public ProfileKeyActionParameters ActionParameters { get; }

        public HotkeyDefinition(uint hotkeyModifiers, uint hotkeyKey, ProfileKeyActionParameters actionParameters)
        {
            HotkeyModifiers = hotkeyModifiers;
            HotkeyKey = hotkeyKey;
            ActionParameters = actionParameters;
        }
    }
}
