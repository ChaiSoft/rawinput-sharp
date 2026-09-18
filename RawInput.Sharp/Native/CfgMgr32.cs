using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.Devices.DeviceAndDriverInstallation;

namespace Linearstar.Windows.RawInput.Native;

[SupportedOSPlatform("windows6.0.6000")]
internal static partial class CfgMgr32
{
    public static DeviceInstanceHandle LocateDevNode(string devicePath, CM_LOCATE_DEVNODE_FLAGS flags)
    {
        TryLocateDevNode(devicePath, flags, out var device).EnsureSuccess();

        return device;
    }

    public static ConfigReturnValue TryLocateDevNode(string devicePath, CM_LOCATE_DEVNODE_FLAGS flags, out DeviceInstanceHandle device)
    {
        unsafe
        {
            fixed(char* pDevicePath = devicePath)
            {
                var result = PInvoke.CM_Locate_DevNode(out var devInst, pDevicePath, flags);

                device = result == ConfigReturnValue.CR_SUCCESS
                    ? (DeviceInstanceHandle)devInst
                    : DeviceInstanceHandle.Zero;

                return result;
            }
        }
        
    }

    public static string? GetDevNodePropertyString(DeviceInstanceHandle device, in DevicePropertyKey propertyKey)
    {
        TryGetDevNodePropertyString(device, in propertyKey, out var value);

        return value;
    }

    private const ushort MAX_STACK = 4096;

    public static ConfigReturnValue TryGetDevNodePropertyString(DeviceInstanceHandle device, in DevicePropertyKey propertyKey, out string? value)
    {
        var devInst = DeviceInstanceHandle.GetRawValue(device);
        uint size = 0;

        var result = PInvoke.CM_Get_DevNode_Property(devInst, in propertyKey, out _, default, ref size, 0);
        if (result != ConfigReturnValue.CR_SUCCESS &&
            result != ConfigReturnValue.CR_BUFFER_SMALL)
        {
            value = null;
            return result;
        }

        var buffer = size <= MAX_STACK ? stackalloc byte[(int)size] : new byte[size];
        result = PInvoke.CM_Get_DevNode_Property(devInst, in propertyKey, out _, buffer, ref size, 0);
        if (result != ConfigReturnValue.CR_SUCCESS)
            value = null;
        else
            value = MarshalEx.PtrToStringUni(buffer);

        return result;
    }

    static void EnsureSuccess(this ConfigReturnValue result)
    {
        if (result != ConfigReturnValue.CR_SUCCESS) throw new InvalidOperationException(result.ToString());
    }
}
