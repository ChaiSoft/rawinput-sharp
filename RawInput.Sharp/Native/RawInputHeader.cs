global using RawInputHeader = Windows.Win32.UI.Input.RAWINPUTHEADER;

namespace Windows.Win32.UI.Input;

internal partial struct RAWINPUTHEADER
{
    public override string ToString() =>
        $"{{{dwType}: {hDevice}, WParam: {wParam}}}";
}

//using System;
//using System.Runtime.InteropServices;
//using Windows.Win32.UI.Input;

//namespace Linearstar.Windows.RawInput.Native;

///// <summary>
///// RAWINPUTHEADER
///// </summary>
//[StructLayout(LayoutKind.Sequential)]
//public readonly struct RawInputHeader
//{

//    readonly RawInputDeviceType dwType;
//    readonly int dwSize;
//    readonly RawInputDeviceHandle hDevice;
//    readonly IntPtr wParam;

//    public RawInputDeviceType Type => dwType;
//    public int Size => dwSize;
//    public RawInputDeviceHandle DeviceHandle => hDevice;
//    public IntPtr WParam => wParam;

//    public override string ToString() =>
//        $"{{{Type}: {DeviceHandle}, WParam: {WParam}}}";
//}