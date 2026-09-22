using System;
using System.Runtime.InteropServices;
using Linearstar.Windows.RawInput.Native;

namespace Linearstar.Windows.RawInput;

public class RawInputKeyboardData : RawInputData
{
    internal RawKeyboard Keyboard { get; }

    internal RawInputKeyboardData(RawInputHeader header, RawKeyboard keyboard)
        : base(header) =>
        Keyboard = keyboard;

    public override unsafe int Length => sizeof(RawInputHeader) + sizeof(RawKeyboard);

    public override bool TryWrite(Span<byte> buffer)
    {
        var header = Header;
        var kb = Keyboard;
        if(!MemoryMarshal.TryWrite(buffer, ref header)) return false;
        return MemoryMarshal.TryWrite(buffer[HEADER_LENGTH..], ref kb);
    }
    public override string ToString() =>
        $"{{{Header}, {Keyboard}}}";
}