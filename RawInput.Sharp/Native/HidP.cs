using System;
using System.Runtime.InteropServices;
using Windows.Win32;

namespace Linearstar.Windows.RawInput.Native;

internal static partial class HidP
{
    //[LibraryImport("hid")]
    //private static partial NtStatus HidP_GetCaps(IntPtr preparsedData, IntPtr capabilities);

    //[LibraryImport("hid")]
    //private static partial NtStatus HidP_GetButtonCaps(HidPReportType reportType, IntPtr buttonCaps, ref ushort buttonCapsLength, IntPtr preparsedData);

    //[LibraryImport("hid")]
    //private static partial NtStatus HidP_GetValueCaps(HidPReportType reportType, IntPtr valueCaps, ref ushort valueCapsLength, IntPtr preparsedData);

    //[LibraryImport("hid")]
    //private static partial NtStatus HidP_GetUsages(HidPReportType reportType, ushort usagePage, ushort linkCollection, IntPtr usageList, ref uint usageLength, IntPtr preparsedData, IntPtr report, uint reportLength);

    //[LibraryImport("hid")]
    //private static partial NtStatus HidP_GetUsageValue(HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, out int usageValue, IntPtr preparsedData, IntPtr report, uint reportLength);

    //[LibraryImport("hid")]
    //private static partial NtStatus HidP_GetScaledUsageValue(HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, out int usageValue, IntPtr preparsedData, IntPtr report, uint reportLength);

    //[LibraryImport("hid")]
    //private static partial NtStatus HidP_GetUsageValueArray(HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, IntPtr usageValue, ushort usageValueByteLength, IntPtr preparsedData, IntPtr report, uint reportLength);
    private const ushort MAX_STACK = 4096;

    private static NtStatus TryGetCaps(HidPreparsedData preparsedData, out HidPCaps capabilities) => PInvoke.HidP_GetCaps(preparsedData, out capabilities);

    public static HidPCaps GetCaps(HidPreparsedData preparsedData)
    {
        TryGetCaps(preparsedData, out var capabilities).EnsureSuccess();

        return capabilities;
    }

    public static HidPCaps GetCaps(IntPtr preparsedData) => GetCaps((HidPreparsedData)preparsedData);

    public static NtStatus TryGetButtonCaps(HidPreparsedData preparsedData, HidPReportType reportType, out HidPButtonCaps[] buttonCaps)
    {
        var caps = GetCaps(preparsedData);
        var capsCount = reportType switch
        {
            HidPReportType.HidP_Input => caps.NumberInputButtonCaps,
            HidPReportType.HidP_Output => caps.NumberOutputButtonCaps,
            HidPReportType.HidP_Feature => caps.NumberFeatureButtonCaps,
            _ => throw new ArgumentException($"Invalid HidPReportType: {reportType}", nameof(reportType)),
        };

        buttonCaps = new HidPButtonCaps[capsCount];
        var result = PInvoke.HidP_GetButtonCaps(reportType, buttonCaps, ref capsCount, preparsedData);

        if (result != NtStatus.HIDP_STATUS_SUCCESS)
            buttonCaps = Array.Empty<HidPButtonCaps>();

        return result;
    }
    
    public static NtStatus TryGetButtonCaps(IntPtr preparsedData, HidPReportType reportType, out HidPButtonCaps[] buttonCaps) =>
        TryGetButtonCaps((HidPreparsedData)preparsedData, reportType, out buttonCaps);

    public static HidPButtonCaps[] GetButtonCaps(HidPreparsedData preparsedData, HidPReportType reportType)
    {
        TryGetButtonCaps(preparsedData, reportType, out var buttonCaps).EnsureSuccess();

        return buttonCaps;
    }
    
    public static HidPButtonCaps[] GetButtonCaps(IntPtr preparsedData, HidPReportType reportType) =>
        GetButtonCaps((HidPreparsedData)preparsedData, reportType);

    public static NtStatus TryGetValueCaps(HidPreparsedData preparsedData, HidPReportType reportType, out HidPValueCaps[] valueCaps)
    {
        var caps = GetCaps(preparsedData);
        var capsCount = reportType switch
        {
            HidPReportType.HidP_Input => caps.NumberInputValueCaps,
            HidPReportType.HidP_Output => caps.NumberOutputValueCaps,
            HidPReportType.HidP_Feature => caps.NumberFeatureValueCaps,
            _ => throw new ArgumentException($"Invalid HidPReportType: {reportType}", nameof(reportType)),
        };

        valueCaps = new HidPValueCaps[capsCount];
        var result = PInvoke.HidP_GetValueCaps(reportType, valueCaps, ref capsCount, preparsedData);

        if (result != NtStatus.HIDP_STATUS_SUCCESS)
            valueCaps = Array.Empty<HidPValueCaps>();

        return result;
    }
    
    public static NtStatus TryGetValueCaps(IntPtr preparsedData, HidPReportType reportType, out HidPValueCaps[] valueCaps) =>
        TryGetValueCaps((HidPreparsedData)preparsedData, reportType, out valueCaps);

    public static HidPValueCaps[] GetValueCaps(HidPreparsedData preparsedData, HidPReportType reportType)
    {
        TryGetValueCaps(preparsedData, reportType, out var valueCaps).EnsureSuccess();

        return valueCaps;
    }
    
    public static HidPValueCaps[] GetValueCaps(IntPtr preparsedData, HidPReportType reportType) =>
        GetValueCaps((HidPreparsedData)preparsedData, reportType);

    public static unsafe NtStatus TryGetUsages(HidPreparsedData preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, byte[] report, uint reportLength, out ushort[] usageList)
    {

        fixed (byte* reportBuffer = report)
        {
            uint usageCount = 0;
            PInvoke.HidP_GetUsages(reportType, usagePage, linkCollection, null, &usageCount, preparsedData, reportBuffer, reportLength);

            usageList = new ushort[usageCount];

            return PInvoke.HidP_GetUsages(reportType, usagePage, linkCollection, usageList, ref usageCount, preparsedData, reportBuffer, reportLength);
        }
    }
    
    public static NtStatus TryGetUsages(IntPtr preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, byte[] report, uint reportLength, out ushort[] usageList) =>
        TryGetUsages((HidPreparsedData)preparsedData, reportType, usagePage, linkCollection, report, reportLength, out usageList);

    public static NtStatus TryGetUsages(HidPreparsedData preparsedData, HidPReportType reportType, HidPButtonCaps buttonCaps, byte[] report, uint reportLength, out ushort[] usageList) =>
        TryGetUsages(preparsedData, reportType, buttonCaps.UsagePage, buttonCaps.LinkCollection, report, reportLength, out usageList);

    public static NtStatus TryGetUsages(IntPtr preparsedData, HidPReportType reportType, HidPButtonCaps buttonCaps, byte[] report, uint reportLength, out ushort[] usageList) =>
        TryGetUsages(preparsedData, reportType, buttonCaps.UsagePage, buttonCaps.LinkCollection, report, reportLength, out usageList);
    
    public static ushort[] GetUsages(HidPreparsedData preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, byte[] report, uint reportLength)
    {
        TryGetUsages(preparsedData, reportType, usagePage, linkCollection, report, reportLength, out var usageList).EnsureSuccess();

        return usageList;
    }
    
    public static ushort[] GetUsages(IntPtr preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, byte[] report, uint reportLength) =>
        GetUsages((HidPreparsedData)preparsedData, reportType, usagePage, linkCollection, report, reportLength);

    public static ushort[] GetUsages(HidPreparsedData preparsedData, HidPReportType reportType, HidPButtonCaps buttonCaps, byte[] report, uint reportLength) =>
        GetUsages(preparsedData, reportType, buttonCaps.UsagePage, buttonCaps.LinkCollection, report, reportLength);

    public static ushort[] GetUsages(IntPtr preparsedData, HidPReportType reportType, HidPButtonCaps buttonCaps, byte[] report, uint reportLength) =>
        GetUsages(preparsedData, reportType, buttonCaps.UsagePage, buttonCaps.LinkCollection, report, reportLength);
    
    public static unsafe NtStatus TryGetUsageValue(HidPreparsedData preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, byte[] report, uint reportLength, out uint usageValue)
    {
        fixed (byte* reportBuffer = report)
            return PInvoke.HidP_GetUsageValue(reportType, usagePage, linkCollection, usage, out usageValue, preparsedData, reportBuffer, reportLength);
    }

    public static NtStatus TryGetUsageValue(IntPtr preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, byte[] report, uint reportLength, out uint usageValue) =>
        TryGetUsageValue((HidPreparsedData)preparsedData, reportType, usagePage, linkCollection, usage, report, reportLength, out usageValue);
    
    public static NtStatus TryGetUsageValue(HidPreparsedData preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, uint reportLength, out uint usageValue) =>
        TryGetUsageValue(preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, report, reportLength, out usageValue);

    public static NtStatus TryGetUsageValue(IntPtr preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, uint reportLength, out uint usageValue) =>
        TryGetUsageValue(preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, report, reportLength, out usageValue);
    
    public static uint GetUsageValue(HidPreparsedData preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, byte[] report, uint reportLength)
    {
        TryGetUsageValue(preparsedData, reportType, usagePage, linkCollection, usage, report, reportLength, out var usageValue).EnsureSuccess();

        return usageValue;
    }
    
    public static uint GetUsageValue(IntPtr preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, byte[] report, int reportLength) =>
        GetUsageValue((HidPreparsedData)preparsedData, reportType, usagePage, linkCollection, usage, report, reportLength);

    public static uint GetUsageValue(HidPreparsedData preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, uint reportLength) =>
        GetUsageValue(preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, report, reportLength);

    public static uint GetUsageValue(IntPtr preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, uint reportLength) =>
        GetUsageValue((HidPreparsedData)preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, report, reportLength);

    public static unsafe NtStatus TryGetScaledUsageValue(HidPreparsedData preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, byte[] report, uint reportLength, out int usageValue)
    {
        fixed (byte* reportBuffer = report)
            return PInvoke.HidP_GetScaledUsageValue(reportType, usagePage, linkCollection, usage, out usageValue, preparsedData, reportBuffer, reportLength);
    }

    public static NtStatus TryGetScaledUsageValue(IntPtr preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, byte[] report, uint reportLength, out int usageValue) =>
        TryGetScaledUsageValue((HidPreparsedData)preparsedData, reportType, usagePage, linkCollection, usage, report, reportLength, out usageValue);

    public static NtStatus TryGetScaledUsageValue(HidPreparsedData preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, uint reportLength, out int usageValue) =>
        TryGetScaledUsageValue(preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, report, reportLength, out usageValue);
    
    public static NtStatus TryGetScaledUsageValue(IntPtr preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, uint reportLength, out int usageValue) =>
        TryGetScaledUsageValue(preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, report, reportLength, out usageValue);

    public static int GetScaledUsageValue(HidPreparsedData preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, byte[] report, uint reportLength)
    {
        TryGetScaledUsageValue(preparsedData, reportType, usagePage, linkCollection, usage, report, reportLength, out var usageValue).EnsureSuccess();

        return usageValue;
    }
    
    public static int GetScaledUsageValue(IntPtr preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, byte[] report, uint reportLength) =>
        GetScaledUsageValue((HidPreparsedData)preparsedData, reportType, usagePage, linkCollection, usage, report, reportLength);

    public static int GetScaledUsageValue(HidPreparsedData preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, uint reportLength) =>
        GetScaledUsageValue(preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, report, reportLength);

    public static int GetScaledUsageValue(IntPtr preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, uint reportLength) =>
        GetScaledUsageValue(preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, report, reportLength);
    
    public static unsafe NtStatus TryGetUsageValueArray(HidPreparsedData preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, ushort usageValueByteLength, byte[] report, int reportLength, out byte[] usageValue)
    {
        usageValue = new byte[usageValueByteLength];

        fixed (byte* usageValueBuffer = usageValue)
        fixed (byte* reportBuffer = report)
            return PInvoke.HidP_GetUsageValueArray(reportType, usagePage, linkCollection, usage, usageValueBuffer, usageValueByteLength, preparsedData, reportBuffer, (uint)reportLength);
    }
    
    public static NtStatus TryGetUsageValueArray(IntPtr preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, ushort usageValueByteLength, byte[] report, uint reportLength, out byte[] usageValue) =>
        TryGetUsageValueArray((HidPreparsedData)preparsedData, reportType, usagePage, linkCollection, usage, usageValueByteLength, report, reportLength, out usageValue);

    public static NtStatus TryGetUsageValueArray(HidPreparsedData preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, uint reportLength, out byte[] usageValue) =>
        TryGetUsageValueArray(preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, (ushort)(valueCaps.BitSize * valueCaps.ReportCount), report, reportLength, out usageValue);
    
    public static NtStatus TryGetUsageValueArray(IntPtr preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, uint reportLength, out byte[] usageValue) =>
        TryGetUsageValueArray(preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, (ushort)(valueCaps.BitSize * valueCaps.ReportCount), report, reportLength, out usageValue);

    public static byte[] GetUsageValueArray(HidPreparsedData preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, ushort usageValueByteLength, byte[] report, uint reportLength)
    {
        TryGetUsageValueArray(preparsedData, reportType, usagePage, linkCollection, usage, usageValueByteLength, report, reportLength, out var usageValue).EnsureSuccess();

        return usageValue;
    }
    
    public static byte[] GetUsageValueArray(IntPtr preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, ushort usageValueByteLength, byte[] report, uint reportLength) => 
        GetUsageValueArray((HidPreparsedData)preparsedData, reportType, usagePage, linkCollection, usage, usageValueByteLength, report, reportLength);

    public static byte[] GetUsageValueArray(HidPreparsedData preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, uint reportLength) =>
        GetUsageValueArray(preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, (ushort)(valueCaps.BitSize * valueCaps.ReportCount), report, reportLength);

    public static byte[] GetUsageValueArray(IntPtr preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, uint reportLength) =>
        GetUsageValueArray(preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, (ushort)(valueCaps.BitSize * valueCaps.ReportCount), report, reportLength);
    
    public static void EnsureSuccess(this NtStatus result)
    {
        if (result != NtStatus.HIDP_STATUS_SUCCESS) throw new InvalidOperationException(result.ToString());
    }
}