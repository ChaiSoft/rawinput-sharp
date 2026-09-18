using System;
using System.Globalization;
using Linearstar.Windows.RawInput.Native;
using Windows.Win32.UI.Input;

namespace Linearstar.Windows.RawInput;

public class RawInputKeyboard : RawInputDevice
{
    public override HidUsageAndPage UsageAndPage => HidUsageAndPage.Keyboard;

    public override uint VendorId =>
        DevicePath?.Contains("VID_") == true
            ? uint.Parse(DevicePath.Substring(DevicePath.IndexOf("VID_", StringComparison.Ordinal) + 4, 4), NumberStyles.HexNumber)
            : 0;

    public override uint ProductId =>
        DevicePath?.Contains("PID_") == true
            ? uint.Parse(DevicePath.Substring(DevicePath.IndexOf("PID_", StringComparison.Ordinal) + 4, 4), NumberStyles.HexNumber)
            : 0;

    public uint KeyboardType => DeviceInfo.keyboard.dwType;
    public uint KeyboardSubType => DeviceInfo.keyboard.dwSubType;
    public uint KeyboardMode => DeviceInfo.keyboard.dwKeyboardMode;
    public uint FunctionKeyCount => DeviceInfo.keyboard.dwNumberOfFunctionKeys;
    public uint IndicatorCount => DeviceInfo.keyboard.dwNumberOfIndicators;
    public uint TotalKeyCount => DeviceInfo.keyboard.dwNumberOfKeysTotal;

    internal RawInputKeyboard(RawInputDeviceHandle device, RawInputDeviceInfo deviceInfo)
        : base(device, deviceInfo)
    {
        if (deviceInfo.dwType != RID_DEVICE_INFO_TYPE.RIM_TYPEKEYBOARD) throw new ArgumentException($"Device type must be {RawInputDeviceType.Keyboard}", nameof(deviceInfo));
    }
}