using System;
using Linearstar.Windows.RawInput.Native;
using Windows.Win32.UI.Input;

namespace Linearstar.Windows.RawInput;

public class RawInputHid : RawInputDevice
{
    readonly Lazy<HidReader> hidReader;

    public override HidUsageAndPage UsageAndPage => DeviceInfo.hid.UsageAndPage;

    public override uint VendorId => DeviceInfo.hid.dwVendorId;

    public override uint ProductId => DeviceInfo.hid.dwProductId;

    public uint Version => DeviceInfo.hid.dwVersionNumber;

    public HidReader Reader => hidReader.Value;

    internal RawInputHid(RawInputDeviceHandle device, RawInputDeviceInfo deviceInfo)
        : base(device, deviceInfo)
    {
        if (deviceInfo.dwType != RID_DEVICE_INFO_TYPE.RIM_TYPEHID) throw new ArgumentException($"Device type must be {RawInputDeviceType.Hid}.", nameof(deviceInfo));

        hidReader = new Lazy<HidReader>(() => new HidReader(new HidPreparsedByteArrayData(GetPreparsedData())));
    }
}