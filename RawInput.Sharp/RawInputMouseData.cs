using System;
using System.Runtime.InteropServices;
using Linearstar.Windows.RawInput.Native;

namespace Linearstar.Windows.RawInput;

public class RawInputMouseData : RawInputData
{
    internal RawMouse Mouse { get; }

    internal RawInputMouseData(RawInputHeader header, RawMouse mouse)
        : base(header) =>
        Mouse = mouse;

    public override unsafe int Length => sizeof(RawInputHeader) + sizeof(RawMouse);
    public override bool TryWrite(Span<byte> buffer)
    {
        var header = Header;
        var ms = Mouse;
        if (!MemoryMarshal.TryWrite(buffer, ref header)) return false;
        return MemoryMarshal.TryWrite(buffer[HEADER_LENGTH..], ref ms);
    }

    public override string ToString() =>
        $"{{{Header}, {Mouse}}}";
}