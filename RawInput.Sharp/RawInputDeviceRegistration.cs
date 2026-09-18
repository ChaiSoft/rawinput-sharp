using System;

namespace Linearstar.Windows.RawInput
{

    public readonly struct RawInputDeviceRegistration
    {
        readonly ushort usUsagePage;
        readonly ushort usUsage;
        readonly RawInputDeviceFlags dwFlags;
        readonly IntPtr hwndTarget;

        public ushort UsagePage => usUsagePage;
        public ushort Usage => usUsage;
        public RawInputDeviceFlags Flags => dwFlags;
        public IntPtr HwndTarget => hwndTarget;

        public RawInputDeviceRegistration(HidUsageAndPage usageAndPage, RawInputDeviceFlags flags, IntPtr hWndTarget)
            : this(usageAndPage.UsagePage, usageAndPage.Usage, flags, hWndTarget)
        {
        }

        public RawInputDeviceRegistration(ushort usagePage, ushort usage, RawInputDeviceFlags flags, IntPtr hWndTarget)
        {
            usUsagePage = usagePage;
            usUsage = usage;
            dwFlags = flags;
            hwndTarget = hWndTarget;
        }
    }
}

namespace Windows.Win32.UI.Input
{
    using Linearstar.Windows.RawInput;
    using Windows.Win32.Foundation;

    internal partial struct RAWINPUTDEVICE
    {
        public static implicit operator RawInputDeviceRegistration(RAWINPUTDEVICE input)
            => new RawInputDeviceRegistration(input.usUsagePage, input.usUsage, input.dwFlags.FromNative(), input.hwndTarget);
        public static implicit operator RAWINPUTDEVICE(RawInputDeviceRegistration input)
            => new RAWINPUTDEVICE() { usUsagePage = input.UsagePage, usUsage = input.Usage, dwFlags = input.Flags.ToWindows(), hwndTarget = (HWND)input.HwndTarget };
    }
}