global using HidPreparsedData = Windows.Win32.Devices.HumanInterfaceDevice.PHIDP_PREPARSED_DATA;

namespace Windows.Win32.Devices.HumanInterfaceDevice
{
    using Linearstar.Windows.RawInput;
    internal readonly partial struct PHIDP_PREPARSED_DATA : IHidPreparsedData
    {
        public unsafe ref byte GetPinnableReference() => ref *(byte*)Value;
    }
}

//using System;

//namespace Linearstar.Windows.RawInput.Native;

///// <summary>
///// HIDP_PREPARSED_DATA
///// </summary>
//public readonly struct HidPreparsedData : IHidPreparsedData, IEquatable<HidPreparsedData>
//{
//    readonly IntPtr value;

//    public static HidPreparsedData Zero => (HidPreparsedData)IntPtr.Zero;

//    HidPreparsedData(IntPtr value)
//    {
//        this.value = value;
//    }

//    public static explicit operator HidPreparsedData(IntPtr value) => new(value);

//    public static explicit operator IntPtr(HidPreparsedData preparsedData) => preparsedData.value;

//    public static bool operator ==(HidPreparsedData a, HidPreparsedData b) => a.Equals(b);

//    public static bool operator !=(HidPreparsedData a, HidPreparsedData b) => !a.Equals(b);

//    public bool Equals(HidPreparsedData other) => value.Equals(other.value);

//    public override bool Equals(object? obj) =>
//        obj is HidPreparsedData other &&
//        Equals(other);

//    public override int GetHashCode() => value.GetHashCode();

//    public override string ToString() => value.ToString();

//    public unsafe ref byte GetPinnableReference() => ref *(byte*)value.ToPointer();
//}