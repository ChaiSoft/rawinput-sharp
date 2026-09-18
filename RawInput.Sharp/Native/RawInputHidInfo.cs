namespace Windows.Win32.UI.Input
{
    using Linearstar.Windows.RawInput;
    internal partial struct RID_DEVICE_INFO_HID
    {
        public HidUsageAndPage UsageAndPage => new HidUsageAndPage(usUsagePage, usUsage);
    }
}

//using System.Runtime.InteropServices;

//namespace Linearstar.Windows.RawInput.Native;

///// <summary>
///// RID_DEVICE_INFO_HID
///// </summary>
//[StructLayout(LayoutKind.Sequential)]
//public readonly struct RawInputHidInfo
//{
//    readonly int dwVendorId;
//    readonly int dwProductId;
//    readonly int dwVersionNumber;
//    readonly ushort usUsagePage;
//    readonly ushort usUsage;

//    /// <summary>
//    /// dwVendorId
//    /// </summary>
//    public int VendorId => dwVendorId;

//    /// <summary>
//    /// dwProductId
//    /// </summary>
//    public int ProductId => dwProductId;

//    /// <summary>
//    /// dwVersionNumber
//    /// </summary>
//    public int VersionNumber => dwVersionNumber;

//    /// <summary>
//    /// usUsagePage, usUsage
//    /// </summary>
//    public HidUsageAndPage UsageAndPage => new(usUsagePage, usUsage);
//}