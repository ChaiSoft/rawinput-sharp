using System;
using Linearstar.Windows.RawInput.Native;

namespace Linearstar.Windows.RawInput;

public class HidValueState
{
    readonly byte[] report;
    readonly uint reportLength;

    public HidValue Value { get; }

    public unsafe uint CurrentValue
    {
        get
        {
            fixed (void* preparsedData = Value.reader.PreparsedData)
                return HidP.GetUsageValue((IntPtr)preparsedData, HidPReportType.HidP_Input, Value.valueCaps, Value.UsageAndPage.Usage, report, reportLength);
        }
    }

    public unsafe int? ScaledValue
    {
        get
        {
            fixed (void* preparsedData = Value.reader.PreparsedData)
                return HidP.TryGetScaledUsageValue((IntPtr)preparsedData, HidPReportType.HidP_Input, Value.valueCaps, Value.UsageAndPage.Usage, report, reportLength, out var value) == NtStatus.HIDP_STATUS_SUCCESS
                    ? value
                    : null;
        }
    }

    public bool HasValue
    {
        get
        {
            if (!Value.CanBeNull) return true;

            var currentValue = CurrentValue;

            return currentValue >= Value.MinValue && currentValue <= Value.MaxValue;
        }
    }

    internal HidValueState(HidValue value, byte[] report, uint reportLength)
    {
        Value = value;
        this.report = report;
        this.reportLength = reportLength;
    }

    public override string ToString() =>
        $"Value: {{{Value}}}, CurrentValue: {CurrentValue}";
}