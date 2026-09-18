using System;

namespace Linearstar.Windows.RawInput.Native;

/// <summary>
/// DEVINST
/// </summary>
public readonly struct DeviceInstanceHandle : IEquatable<DeviceInstanceHandle>
{
    readonly uint value;

    public static DeviceInstanceHandle Zero => (DeviceInstanceHandle)0U;

    DeviceInstanceHandle(uint value) => this.value = value;

    public static uint GetRawValue(DeviceInstanceHandle handle) => handle.value;

    public static explicit operator DeviceInstanceHandle(uint value) => new(value);

    public static bool operator ==(DeviceInstanceHandle a, DeviceInstanceHandle b) => a.Equals(b);

    public static bool operator !=(DeviceInstanceHandle a, DeviceInstanceHandle b) => !a.Equals(b);

    public bool Equals(DeviceInstanceHandle other) => value.Equals(other.value);

    public override bool Equals(object? obj) =>
        obj is DeviceInstanceHandle other &&
        Equals(other);

    public override int GetHashCode() => value.GetHashCode();

    public override string ToString() => value.ToString();
}
