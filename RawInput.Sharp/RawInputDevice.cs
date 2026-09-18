using System;
using System.Linq;
using Linearstar.Windows.RawInput.Native;
using Windows.Win32.Devices.DeviceAndDriverInstallation;
using Windows.Win32.UI.Input;

namespace Linearstar.Windows.RawInput;

public abstract class RawInputDevice
{
    bool gotAttributes;
    string? productName;
    string? manufacturerName;
    string? serialNumber;

    private protected RawInputDeviceInfo DeviceInfo { get; }

    public RawInputDeviceHandle Handle { get; }
    public RawInputDeviceType DeviceType => DeviceInfo.dwType.FromNative();
    public string? DevicePath { get; }

    public string? ManufacturerName
    {
        get
        {
            if (manufacturerName == null) GetAttributesOnce();
            return manufacturerName;
        }
    }

    public string? ProductName
    {
        get
        {
            if (productName == null) GetAttributesOnce();
            return productName;
        }
    }

    public string? SerialNumber
    {
        get
        {
            if (serialNumber == null) GetAttributesOnce();
            return serialNumber;
        }
    }

    public bool IsConnected =>
        DevicePath != null && CfgMgr32.TryLocateDevNode(DevicePath, CM_LOCATE_DEVNODE_FLAGS.CM_LOCATE_DEVNODE_NORMAL, out _) == ConfigReturnValue.CR_SUCCESS;

    public abstract HidUsageAndPage UsageAndPage { get; }
    public abstract uint VendorId { get; }
    public abstract uint ProductId { get; }

    void GetAttributesOnce()
    {
        if (gotAttributes) return;
        gotAttributes = true;

        if (DevicePath == null) return;
        GetAttributesFromHidD();
        if (manufacturerName == null || productName == null) GetAttributesFromCfgMgr();
    }

    void GetAttributesFromHidD()
    {
        if (DevicePath == null || !HidD.TryOpenDevice(DevicePath, out var device)) return;

        try
        {
            manufacturerName ??= HidD.GetManufacturerString(device);
            productName ??= HidD.GetProductString(device);
            serialNumber ??= HidD.GetSerialNumberString(device);
        }
        finally
        {
            HidD.CloseDevice(device);
        }
    }

    void GetAttributesFromCfgMgr()
    {
        if (DevicePath == null) return;

        var path = DevicePath.Substring(4).Replace('#', '\\');
        if (path.Contains("{")) path = path.Substring(0, path.IndexOf('{') - 1);

        var device = CfgMgr32.LocateDevNode(path, CM_LOCATE_DEVNODE_FLAGS.CM_LOCATE_DEVNODE_PHANTOM);

        manufacturerName ??= CfgMgr32.GetDevNodePropertyString(device, in DevicePropertyKey.DeviceManufacturer);
        productName ??= CfgMgr32.GetDevNodePropertyString(device, in DevicePropertyKey.DeviceFriendlyName);
        productName ??= CfgMgr32.GetDevNodePropertyString(device, in DevicePropertyKey.Name);
    }

    private protected RawInputDevice(RawInputDeviceHandle device, RawInputDeviceInfo deviceInfo)
    {
        Handle = device;
        DevicePath = User32.GetRawInputDeviceName(device);
        DeviceInfo = deviceInfo;
    }

    public static RawInputDevice FromHandle(RawInputDeviceHandle device)
    {
        var deviceInfo = User32.GetRawInputDeviceInfo(device);

        switch (deviceInfo.dwType)
        {
            case RID_DEVICE_INFO_TYPE.RIM_TYPEMOUSE:
                return new RawInputMouse(device, deviceInfo);
            case RID_DEVICE_INFO_TYPE.RIM_TYPEKEYBOARD:
                return new RawInputKeyboard(device, deviceInfo);
            case RID_DEVICE_INFO_TYPE.RIM_TYPEHID:
                return RawInputDigitizer.IsSupported(deviceInfo.hid.UsageAndPage)
                    ? new RawInputDigitizer(device, deviceInfo)
                    : new RawInputHid(device, deviceInfo);
            default:
                throw new ArgumentException();
        }
    }

    /// <summary>
    /// Gets available devices that can be handled with Raw Input.
    /// </summary>
    /// <returns>Array of <see cref="RawInputDevice"/>, which contains mouse as a <see cref="RawInputMouse"/>, keyboard as a <see cref="RawInputKeyboard"/>, and any other HIDs as a <see cref="RawInputHid"/>.</returns>
    public static RawInputDevice[] GetDevices()
    {
        var devices = User32.GetRawInputDeviceList();

        return devices.Select(i => FromHandle((RawInputDeviceHandle)i.hDevice)).ToArray();
    }

    public byte[] GetPreparsedData() =>
        User32.GetRawInputDevicePreparsedData(Handle);

    public static void RegisterDevice(HidUsageAndPage usageAndPage, RawInputDeviceFlags flags, IntPtr hWndTarget) =>
        RegisterDevice(new RawInputDeviceRegistration(usageAndPage, flags, hWndTarget));

    public static void RegisterDevice(params RawInputDeviceRegistration[] devices) =>
        User32.RegisterRawInputDevices(devices);

    public static void UnregisterDevice(HidUsageAndPage usageAndPage) =>
        RegisterDevice(usageAndPage, RawInputDeviceFlags.Remove, IntPtr.Zero);

    public static RawInputDeviceRegistration[] GetRegisteredDevices() =>
        User32.GetRegisteredRawInputDevices();
}