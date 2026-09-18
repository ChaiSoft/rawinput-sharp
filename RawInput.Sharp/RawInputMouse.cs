using System;
using System.Globalization;
using Linearstar.Windows.RawInput.Native;
using Windows.Win32.UI.Input;

namespace Linearstar.Windows.RawInput;

public class RawInputMouse : RawInputDevice
{
    public override HidUsageAndPage UsageAndPage => HidUsageAndPage.Mouse;

    public override uint VendorId =>
        DevicePath?.Contains("VID_") == true
            ? uint.Parse(DevicePath.Substring(DevicePath.IndexOf("VID_", StringComparison.Ordinal) + 4, 4), NumberStyles.HexNumber)
            : 0;

    public override uint ProductId =>
        DevicePath?.Contains("PID_") == true
            ? uint.Parse(DevicePath.Substring(DevicePath.IndexOf("PID_", StringComparison.Ordinal) + 4, 4), NumberStyles.HexNumber)
            : 0;

    public uint Id => DeviceInfo.mouse.dwId;
    public uint ButtonCount => DeviceInfo.mouse.dwNumberOfButtons;
    public uint SampleRate => DeviceInfo.mouse.dwSampleRate;
    public bool HasHorizontalWheel => DeviceInfo.mouse.fHasHorizontalWheel;

    internal RawInputMouse(RawInputDeviceHandle device, RawInputDeviceInfo deviceInfo)
        : base(device, deviceInfo)
    {
        if (deviceInfo.dwType != RID_DEVICE_INFO_TYPE.RIM_TYPEMOUSE) throw new ArgumentException($"Device type must be {RawInputDeviceType.Mouse}.", nameof(deviceInfo));
    }
}