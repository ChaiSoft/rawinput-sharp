using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Linearstar.Windows.RawInput.Native;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input;

namespace Linearstar.Windows.RawInput;

public abstract class RawInputData
{
    RawInputDevice? device;

    public RawInputDeviceHandle DeviceHandle => (RawInputDeviceHandle)Header.hDevice;
    internal RawInputHeader Header { get; }
    protected static readonly unsafe int HEADER_LENGTH = sizeof(RawInputHeader);

    public RawInputDevice? Device =>
        device ??= Header.hDevice != HANDLE.Null
            ? RawInputDevice.FromHandle((RawInputDeviceHandle)Header.hDevice)
            : null;

    private protected RawInputData(RawInputHeader header)
    {
        Header = header;
    }

    public static RawInputData FromHandle(IntPtr lParam)
        => FromHandle((RawInputHandle)lParam);

    internal static RawInputData FromHandle(RawInputHandle rawInput)
    {
        var header = User32.GetRawInputDataHeader(rawInput);

        switch ((RID_DEVICE_INFO_TYPE)header.dwType)
        {
            case RID_DEVICE_INFO_TYPE.RIM_TYPEMOUSE:
                return new RawInputMouseData(header, User32.GetRawInputMouseData(rawInput, out _));
            case RID_DEVICE_INFO_TYPE.RIM_TYPEKEYBOARD:
                return new RawInputKeyboardData(header, User32.GetRawInputKeyboardData(rawInput, out _));
            case RID_DEVICE_INFO_TYPE.RIM_TYPEHID:
                return RawInputHidData.Create(header, User32.GetRawInputHidData(rawInput, out _));
            default:
                throw new ArgumentException();
        }
    }

    private const ushort MAX_STACK = 4096;

    private static unsafe RawInputData ParseRawInputBufferItem(ref RAWINPUT ptr)
    {
        
        var header = ptr.header;
        ref var data = ref ptr.data;

        // RAWINPUT structure must be aligned by 8 bytes on WOW64
        // https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getrawinputbuffer#remarks
        if (!EnvironmentEx.Is64BitProcess && EnvironmentEx.Is64BitOperatingSystem)
        {
            data = ref Unsafe.AddByteOffset(ref data, 8);
        }

        switch ((RID_DEVICE_INFO_TYPE)header.dwType)
        {
            case RID_DEVICE_INFO_TYPE.RIM_TYPEMOUSE:
                return new RawInputMouseData(header, data.mouse);
            case RID_DEVICE_INFO_TYPE.RIM_TYPEKEYBOARD:
                return new RawInputKeyboardData(header, data.keyboard);
            case RID_DEVICE_INFO_TYPE.RIM_TYPEHID:
                return RawInputHidData.Create(header, RawHid.FromRef(in data.hid));
            default:
                throw new ArgumentException();
        }
    }

    public static unsafe RawInputData[] GetBufferedData(int length = 8)
    {
        int cbSize;
        unsafe { cbSize = sizeof(RAWINPUT); }

        Span<RAWINPUT> dataBuffer = length * cbSize <= MAX_STACK ? stackalloc RAWINPUT[length] : new RAWINPUT[length];
        uint count = User32.GetRawInputBuffer(dataBuffer);
        count.EnsureSuccess();

        var result = new RawInputData[count];

        ref RAWINPUT ptr = ref dataBuffer[0];
        for (int i = 0; i < count; i++)
        {
            result[i] = ParseRawInputBufferItem(ref ptr);
            ptr = ref Unsafe.AddByteOffset(ref ptr, Align(ptr.header.dwSize));
        }

        return result;
    }

    private static uint Align(uint x) => (x + (uint)UIntPtr.Size - 1U) & ~((uint)UIntPtr.Size - 1U);

    public static void DefRawInputProc(RawInputData[] data)
    {
        int cbSize;
        unsafe { cbSize = sizeof(RAWINPUT); }
        int length = data.Length;
        var native = length * cbSize <= MAX_STACK ? stackalloc RAWINPUT[length] : new RAWINPUT[length];
        //for (int i = 0; i < length; i++)
        //{

        //}
        User32.DefRawInputProc(native);
    }

    public abstract int Length { get; }
    public abstract bool TryWrite(Span<byte> buffer);
    public byte[] ToStructure()
    {
        var data = new byte[Align((uint)Length)];
        if(!TryWrite(data))
            throw new InvalidOperationException();
        return data;
    }
}