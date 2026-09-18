using System;
using System.Runtime.InteropServices;
using System.Text;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Storage.FileSystem;

namespace Linearstar.Windows.RawInput.Native;

internal static partial class HidD
{
    private delegate BOOLEAN HidGetStringFunc(HANDLE HidDeviceObject, Span<byte> buffer);

    public static HidDeviceHandle OpenDevice(string devicePath)
    {
        var deviceHandle = Kernel32.CreateFile(devicePath, FILE_SHARE_MODE.FILE_SHARE_READ | FILE_SHARE_MODE.FILE_SHARE_WRITE, FILE_CREATION_DISPOSITION.OPEN_EXISTING);

        return (HidDeviceHandle)deviceHandle;
    }

    public static bool TryOpenDevice(string devicePath, out HidDeviceHandle device)
    {
        if (!Kernel32.TryCreateFile(
                devicePath,
                FILE_SHARE_MODE.FILE_SHARE_READ | FILE_SHARE_MODE.FILE_SHARE_WRITE,
                FILE_CREATION_DISPOSITION.OPEN_EXISTING,
                out var deviceHandle))
        {
            device = HidDeviceHandle.Zero;
            return false;
        }

        device = (HidDeviceHandle)deviceHandle;
        return true;
    }

    public static void CloseDevice(HidDeviceHandle device)
    {
        Kernel32.CloseHandle(device);
    }

    public static string? GetManufacturerString(HidDeviceHandle device) => GetString(device, PInvoke.HidD_GetManufacturerString);
    public static string? GetProductString(HidDeviceHandle device) => GetString(device, PInvoke.HidD_GetProductString);
    public static string? GetSerialNumberString(HidDeviceHandle device) => GetString(device, PInvoke.HidD_GetSerialNumberString);

    public static HidPreparsedData GetPreparsedData(HidDeviceHandle device)
    {
        PInvoke.HidD_GetPreparsedData(device, out var preparsedData);
        return preparsedData;
    }

    public static void FreePreparsedData(HidPreparsedData preparsedData) => PInvoke.HidD_FreePreparsedData(preparsedData);

    static unsafe string? GetString(HidDeviceHandle handle, HidGetStringFunc proc)
    {
        Span<byte> buf = stackalloc byte[256];
        if (!proc(handle, buf))
            return null;

        return MarshalEx.PtrToStringUni(buf);
    }
}
