using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input;

namespace Linearstar.Windows.RawInput.Native;

[SupportedOSPlatform("windows6.0.6000")]
internal static partial class User32
{

    //[LibraryImport("user32", EntryPoint = "GetRawInputDeviceInfoW", SetLastError = true)]
    //private static partial uint GetRawInputDeviceInfo(IntPtr hDevice, RawInputDeviceInfoBehavior uiBehavior, IntPtr pData, ref uint pcbSize);

    //[LibraryImport("user32", SetLastError = true)]
    //[return: MarshalAs(UnmanagedType.Bool)]
    //private static partial bool RegisterRawInputDevices(IntPtr pRawInputDevices, uint uiNumDevices, uint cbSize);

    //[LibraryImport("user32", SetLastError = true)]
    //private static partial uint GetRegisteredRawInputDevices(IntPtr pRawInputDevices, ref uint puiNumDevices, uint cbSize);

    //[LibraryImport("user32", SetLastError = true)]
    //private static partial uint GetRawInputData(IntPtr hRawInput, RawInputGetBehavior uiBehavior, IntPtr pData, ref uint pcbSize, uint cbSizeHeader);

    //[LibraryImport("user32", SetLastError = true)]
    //private static partial uint GetRawInputBuffer(IntPtr pData, ref uint pcbSize, uint cbSizeHeader);

    //[LibraryImport("user32", SetLastError = true)]
    //private static partial IntPtr DefRawInputProc(IntPtr paRawInput, int nInput, uint cbSizeHeader);

    public enum RawInputGetBehavior : uint
    {
        Input = 0x10000003,
        Header = 0x10000005,
    }

    private const ushort MAX_STACK = 4096;

    private static ushort HEADER_SIZE
    {
        get
        {
            unsafe { return (ushort)sizeof(RawInputHeader); };
        }
    }

    public static unsafe RawInputDeviceListItem[] GetRawInputDeviceList()
    {
        uint size = (uint)sizeof(RawInputDeviceListItem);

        // Get device count by passing null for pRawInputDeviceList.
        uint deviceCount = 0;
        PInvoke.GetRawInputDeviceList(null, &deviceCount, size);

        // Now, fill the buffer using the device count.
        var devices = new RawInputDeviceListItem[deviceCount];
        PInvoke.GetRawInputDeviceList(devices, ref deviceCount, size).EnsureSuccess();

        return devices;
    }

    public static string? GetRawInputDeviceName(RawInputDeviceHandle device)
    {
        // Get the length of the device name first.
        // For RIDI_DEVICENAME, the value in the pcbSize is the character count instead of the byte count.
        uint size = 0;
        PInvoke.GetRawInputDeviceInfo(device, RAW_INPUT_DEVICE_INFO_COMMAND.RIDI_DEVICENAME, default, ref size);

        if (size <= 2) return null;

        Span<byte> buffer = size <= MAX_STACK ? stackalloc byte[(int)size] : new byte[size];
        PInvoke.GetRawInputDeviceInfo(device, RAW_INPUT_DEVICE_INFO_COMMAND.RIDI_DEVICENAME, buffer, ref size).EnsureSuccess();

        return MarshalEx.PtrToStringUni(buffer);
    }

    public static RawInputDeviceInfo GetRawInputDeviceInfo(RawInputDeviceHandle device)
    {
        uint size;
        unsafe { size = (uint)sizeof(RawInputDeviceInfo); }

        Span<RawInputDeviceInfo> buffer = stackalloc RawInputDeviceInfo[1];
        buffer[0].cbSize = size;

        var byteBuffer = MemoryMarshal.AsBytes(buffer);


        PInvoke.GetRawInputDeviceInfo(device, RAW_INPUT_DEVICE_INFO_COMMAND.RIDI_DEVICEINFO, byteBuffer, ref size).EnsureSuccess();

        return buffer[0];
    }

    public static byte[] GetRawInputDevicePreparsedData(RawInputDeviceHandle device)
    {
        uint size = 0;

        PInvoke.GetRawInputDeviceInfo(device, RAW_INPUT_DEVICE_INFO_COMMAND.RIDI_PREPARSEDDATA, default, ref size);

        var result = new byte[size];
        PInvoke.GetRawInputDeviceInfo(device, RAW_INPUT_DEVICE_INFO_COMMAND.RIDI_PREPARSEDDATA, result, ref size).EnsureSuccess();

        return result;
    }

    public static void RegisterRawInputDevices(params ReadOnlySpan<RawInputDeviceRegistration> devices)
    {
        uint cbSize;
        unsafe { cbSize = (uint)sizeof(RAWINPUTDEVICE); }
        int count = devices.Length;
        Span<RAWINPUTDEVICE> native = cbSize * count <= MAX_STACK ? stackalloc RAWINPUTDEVICE[count] : new RAWINPUTDEVICE[count];
        for(int i = 0; i < count; ++i)
            native[i] = devices[i];     //Conversion operator
        PInvoke.RegisterRawInputDevices(native, cbSize).EnsureSuccess();
    }

    public static RawInputDeviceRegistration[] GetRegisteredRawInputDevices()
    {
        uint cbSize;
        unsafe { cbSize = (uint)sizeof(RAWINPUTDEVICE); }

        uint count = 0;
        unsafe { PInvoke.GetRegisteredRawInputDevices(null, &count, cbSize); }

        if (count == 0)
            return Array.Empty<RawInputDeviceRegistration>();

        Span<RAWINPUTDEVICE> native = cbSize * count <= MAX_STACK ? stackalloc RAWINPUTDEVICE[(int)count] : new RAWINPUTDEVICE[count];
        var result = new RawInputDeviceRegistration[count];

        PInvoke.GetRegisteredRawInputDevices(native, ref count, cbSize);

        for (int i = 0; i < count; ++i)
            result[i] = native[i];     //Conversion operator

        return result;
    }

    public static unsafe RawInputHeader GetRawInputDataHeader(RawInputHandle rawInput)
    {
        uint size = HEADER_SIZE;

        Span<RawInputHeader> result = stackalloc RawInputHeader[1];
        var byteResult = MemoryMarshal.AsBytes(result);
        PInvoke.GetRawInputData(rawInput, RAW_INPUT_DATA_COMMAND_FLAGS.RID_HEADER, byteResult, ref size, HEADER_SIZE).EnsureSuccess();

        return result[0];
    }

    private static uint GetRawInputDataSize(RawInputHandle rawInput)
    {
        uint size = 0;

        PInvoke.GetRawInputData(rawInput, RAW_INPUT_DATA_COMMAND_FLAGS.RID_INPUT, default, ref size, HEADER_SIZE).EnsureSuccess();

        return size;
    }

    private static void GetRawInputData(RawInputHandle rawInput, Span<byte> result)
    {
        uint size = (uint)result.Length;
        PInvoke.GetRawInputData(rawInput, RAW_INPUT_DATA_COMMAND_FLAGS.RID_INPUT, result, ref size, HEADER_SIZE).EnsureSuccess();
    }

    public static unsafe RawMouse GetRawInputMouseData(RawInputHandle rawInput, out RawInputHeader header)
    {
        var size = GetRawInputDataSize(rawInput);

        Span<byte> bytes = size <= MAX_STACK ? stackalloc byte[(int)size] : new byte[size];
        GetRawInputData(rawInput, bytes);
        header = MemoryMarshal.Read<RawInputHeader>(bytes);
        return MemoryMarshal.Read<RawMouse>(bytes[HEADER_SIZE..]);
    }

    public static unsafe RawKeyboard GetRawInputKeyboardData(RawInputHandle rawInput, out RawInputHeader header)
    {
        var size = GetRawInputDataSize(rawInput);

        Span<byte> bytes = size <= MAX_STACK ? stackalloc byte[(int)size] : new byte[size];
        GetRawInputData(rawInput, bytes);
        header = MemoryMarshal.Read<RawInputHeader>(bytes);
        return MemoryMarshal.Read<RawKeyboard>(bytes[HEADER_SIZE..]);
    }

    public static unsafe RawHid GetRawInputHidData(RawInputHandle rawInput, out RawInputHeader header)
    {
        var size = GetRawInputDataSize(rawInput);

        Span<byte> bytes = size <= MAX_STACK ? stackalloc byte[(int)size] : new byte[size];
        GetRawInputData(rawInput, bytes);
        header = MemoryMarshal.Read<RawInputHeader>(bytes);
        //RawHid is a special case, as it's variable length
        return RawHid.FromSpan(bytes[HEADER_SIZE..]);
    }

    public static uint GetRawInputBufferSize()
    {
        uint size = 0;

        unsafe { PInvoke.GetRawInputBuffer(null, &size, HEADER_SIZE); }

        return size;
    }

    public static uint GetRawInputBuffer(Span<RAWINPUT> buffer)
    {
        uint size = (uint)MemoryMarshal.AsBytes(buffer).Length;
        return PInvoke.GetRawInputBuffer(buffer, ref size, HEADER_SIZE).EnsureSuccess();
    }

    public static unsafe void DefRawInputProc(byte[] paRawInput)
    {
        throw new NotImplementedException();
        //fixed (byte* buffer = paRawInput)
        //    PInvoke.DefRawInputProc((IntPtr)buffer, paRawInput.Length, HEADER_SIZE);
    }

    public static bool EnsureSuccess(this BOOL result)
    {
        if (!result) throw new Win32ErrorException();

        return result;
    }

    public static uint EnsureSuccess(this uint result)
    {
        if (result == unchecked((uint)-1)) throw new Win32ErrorException();

        return result;
    }
}