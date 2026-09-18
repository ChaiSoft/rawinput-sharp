using System;
using System.Runtime.InteropServices;

namespace Linearstar.Windows.RawInput;

static class MarshalEx
{
#if NET7_0_OR_GREATER
    public static int SizeOf<T>() => Marshal.SizeOf<T>();
#else
    public static int SizeOf<T>() => Marshal.SizeOf(typeof(T));
#endif

    public static string PtrToStringUni(ReadOnlySpan<byte> buffer)
    {
        var unicode = MemoryMarshal.Cast<byte, char>(buffer);
        int nullIndex = unicode.IndexOf('\0');
        if (nullIndex < 0)
            return unicode.ToString();
        else
            return unicode.Slice(0, nullIndex).ToString();
    }
}