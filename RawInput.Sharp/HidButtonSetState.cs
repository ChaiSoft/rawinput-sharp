using System;
using System.Collections;
using System.Collections.Generic;
using Linearstar.Windows.RawInput.Native;

namespace Linearstar.Windows.RawInput;

public class HidButtonSetState : IEnumerable<HidButtonState>
{
    readonly byte[] report;
    readonly uint reportLength;

    public HidButtonSet ButtonSet { get; }

    public unsafe ushort[] ActiveUsages
    {
        get
        {
            fixed (void* preparsedData = ButtonSet.reader.PreparsedData)
                return HidP.GetUsages((nint)preparsedData, HidPReportType.HidP_Input, ButtonSet.buttonCaps, report, reportLength);
        }
    }

    internal HidButtonSetState(HidButtonSet buttonSet, byte[] report, uint reportLength)
    {
        ButtonSet = buttonSet;
        this.report = report;
        this.reportLength = reportLength;
    }

    public override string ToString() =>
        $"ButtonSet: {{{ButtonSet}}}, Active: [{string.Join(", ", ActiveUsages)}]";

    public IEnumerator<HidButtonState> GetEnumerator()
    {
        for (var usage = ButtonSet.UsageMin; usage <= ButtonSet.UsageMax; usage++)
            yield return new HidButtonState(new HidButton(ButtonSet.reader, ButtonSet.buttonCaps, usage), report, reportLength);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}