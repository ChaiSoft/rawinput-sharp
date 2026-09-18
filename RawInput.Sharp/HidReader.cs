using System;
using System.Linq;
using Linearstar.Windows.RawInput.Native;

namespace Linearstar.Windows.RawInput;

public class HidReader
{
    internal readonly HidPCaps capabilities;

    public IHidPreparsedData PreparsedData { get; }
    public int ValueCount => capabilities.NumberInputValueCaps;
    public HidButtonSet[] ButtonSets { get; }
    public HidValueSet[] ValueSets { get; }

    public unsafe HidReader(IHidPreparsedData preparsedData)
    {
        PreparsedData = preparsedData;
        fixed (void* preparsedDataPtr = preparsedData)
        {
            capabilities = HidP.GetCaps((IntPtr)preparsedDataPtr);

            var buttonCaps = HidP.GetButtonCaps((IntPtr)preparsedDataPtr, HidPReportType.HidP_Input);
            ButtonSets = buttonCaps.Select(i => new HidButtonSet(this, i)).ToArray();

            var valueCaps = HidP.GetValueCaps((IntPtr)preparsedDataPtr, HidPReportType.HidP_Input);
            ValueSets = valueCaps.Select(i => new HidValueSet(this, i)).ToArray();
        }
    }
}