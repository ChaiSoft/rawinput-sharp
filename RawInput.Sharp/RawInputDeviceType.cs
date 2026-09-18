using Windows.Win32.UI.Input;

namespace Linearstar.Windows.RawInput;

public enum RawInputDeviceType
{
    Mouse,
    Keyboard,
    Hid,
}

internal static partial class EnumConverter
{
    public static RID_DEVICE_INFO_TYPE ToWindows(this RawInputDeviceType type) => type switch
    {
        RawInputDeviceType.Mouse => RID_DEVICE_INFO_TYPE.RIM_TYPEMOUSE,
        RawInputDeviceType.Keyboard => RID_DEVICE_INFO_TYPE.RIM_TYPEKEYBOARD,
        RawInputDeviceType.Hid => RID_DEVICE_INFO_TYPE.RIM_TYPEHID,
        _ => throw new System.ArgumentException()
    };

    public static RawInputDeviceType FromNative(this RID_DEVICE_INFO_TYPE type) => type switch
    {
        RID_DEVICE_INFO_TYPE.RIM_TYPEMOUSE => RawInputDeviceType.Mouse,
        RID_DEVICE_INFO_TYPE.RIM_TYPEKEYBOARD => RawInputDeviceType.Keyboard,
        RID_DEVICE_INFO_TYPE.RIM_TYPEHID => RawInputDeviceType.Hid,
        _ => throw new System.ArgumentException()
    };
}
