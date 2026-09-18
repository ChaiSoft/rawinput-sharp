using System;
using System.Linq;
using System.Runtime.InteropServices;
using Windows.Win32.UI.Input;

namespace Linearstar.Windows.RawInput.Native;

/// <summary>
/// RAWHID
/// </summary>
public struct RawHid
{
    int dwSizeHid;
    int dwCount;
    byte[] rawData;

    public int ElementSize => dwSizeHid;
    public int Count => dwCount;
    public unsafe byte[] RawData => rawData;

    public static RawHid FromSpan(ReadOnlySpan<byte> span)
    {
        ref readonly RAWHID rawHid = ref MemoryMarshal.Cast<byte, RAWHID>(span)[0];

        var result = new RawHid();
        result.dwSizeHid = checked((int)rawHid.dwSizeHid);
        result.dwCount = checked((int)rawHid.dwCount);
        result.rawData = rawHid.bRawData.AsSpan((int)rawHid.dwCount).ToArray();

        return result;
    }

    public ArraySegment<byte>[] ToHidReports()
    {
        var elementSize = ElementSize;
        var rawDataArray = RawData;

        var result = new ArraySegment<byte>[Count];

        for(int i = 0; i < Count; ++i)
            result[i] = new ArraySegment<byte>(RawData, elementSize * i, elementSize);
        return result;
    }
        
    public unsafe byte[] ToStructure()
    {
        var result = new byte[dwSizeHid * dwCount + sizeof(int) * 2];

        fixed (byte* resultPtr = result)
        {
            var intPtr = (int*)resultPtr;

            intPtr[0] = dwSizeHid;
            intPtr[1] = dwCount;
        }

        rawData.CopyTo(result, sizeof(int) * 2);

        return result;
    }

    public override string ToString() =>
        $"{{Count: {Count}, Size: {ElementSize}, Content: {BitConverter.ToString(RawData).Replace("-", " ")}}}";
}