using System;
using System.Buffers.Binary;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Win32.UI.Input;

namespace Linearstar.Windows.RawInput.Native;

/// <summary>
/// RAWHID
/// </summary>
public struct RawHid
{
    private int dwSizeHid;
    private int dwCount;
    private byte[] rawData;

    public int ElementSize => dwSizeHid;
    public int Count => dwCount;
    public unsafe byte[] RawData => rawData;

    internal static RawHid FromRef(ref readonly RAWHID rawHid)
    {
        var result = new RawHid();
        result.dwSizeHid = checked((int)rawHid.dwSizeHid);
        result.dwCount = checked((int)rawHid.dwCount);
        result.rawData = rawHid.bRawData.AsSpan((int)rawHid.dwCount).ToArray();

        return result;
    }

    public static RawHid FromSpan(ReadOnlySpan<byte> span)
    {
        ref readonly RAWHID rawHid = ref MemoryMarshal.Cast<byte, RAWHID>(span)[0];
        return FromRef(in rawHid);
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


    public int Length => Count * ElementSize + 2 * sizeof(uint);
    public bool TryWrite(Span<byte> span)
    {
        if (span.Length < Length) return false;

        ref byte first = ref span[0];
        ref RAWHID header = ref Unsafe.As<byte, RAWHID>(ref first);
        header.dwSizeHid = (uint)dwSizeHid;
        header.dwCount = (uint)dwCount;
        RawData.CopyTo(span[(2 * sizeof(uint))..]);
        return true;
    }

    public override string ToString() =>
        $"{{Count: {Count}, Size: {ElementSize}, Content: {BitConverter.ToString(RawData).Replace("-", " ")}}}";
}