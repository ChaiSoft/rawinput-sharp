using Linearstar.Windows.RawInput.Native;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using Windows.Win32.Foundation;

namespace Linearstar.Windows.RawInput;

public class RawInputHidData : RawInputData
{
    public new RawInputHid? Device => (RawInputHid?)base.Device;

    public RawHid Hid { get; }

    public HidButtonSetState[] ButtonSetStates =>
        Device != null
            ? Hid.ToHidReports().SelectMany(report => Device.Reader.ButtonSets.Select(x => x.GetStates(report))).ToArray()
            : new HidButtonSetState[0];

    public HidValueSetState[] ValueSetStates =>
        Device != null
            ? Hid.ToHidReports().SelectMany(report => Device.Reader.ValueSets.Select(x => x.GetStates(report))).ToArray()
            : new HidValueSetState[0];

    private protected RawInputHidData(RawInputHeader header, RawHid hid)
        : base(header) =>
        Hid = hid;

    internal static RawInputHidData Create(RawInputHeader header, RawHid hid)
    {
        var device = header.hDevice != HANDLE.Null ? RawInputDevice.FromHandle((RawInputDeviceHandle)header.hDevice) : null;

        if (device != null && RawInputDigitizer.IsSupported(device.UsageAndPage))
            return new RawInputDigitizerData(header, hid);

        return new RawInputHidData(header, hid);
    }

    public override int Length => HEADER_LENGTH + Hid.Length;

    public override bool TryWrite(Span<byte> buffer)
    {
        if (buffer.Length < Length)
            return false;

        var header = Header;
        if (!MemoryMarshal.TryWrite(buffer, in header)) return false;
        return Hid.TryWrite(buffer[HEADER_LENGTH..]);
    }

    //public override unsafe byte[] ToStructure()
    //{
    //    var headerSize = MarshalEx.SizeOf<RawInputHeader>();
    //    var hid = Hid.ToStructure();
    //    var bytes = new byte[Align(headerSize + hid.Length)];

    //    fixed (byte* bytesPtr = bytes)
    //        *(RawInputHeader*) bytesPtr = Header;
            
    //    hid.CopyTo(bytes, headerSize);

    //    return bytes;
    //}

    public override string ToString() =>
        $"{{{Header}, {Hid}}}";
}