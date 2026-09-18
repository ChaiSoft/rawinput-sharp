using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Security;
using Windows.Win32.Storage.FileSystem;
using Windows.Win32.System.Diagnostics.Debug;

namespace Linearstar.Windows.RawInput.Native;

[SupportedOSPlatform("windows6.0.6000")]
internal static partial class Kernel32
{ 
    //[LibraryImport("kernel32", EntryPoint = "CreateFileW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    //private static partial IntPtr CreateFileCore(string lpFileName, DesiredAccess dwDesiredAccess, ShareMode dwShareMode, IntPtr lpSecurityAttributes, CreateDisposition dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

    public static bool CloseHandle(HANDLE hObject) => PInvoke.CloseHandle(hObject);

    //[LibraryImport("kernel32", EntryPoint = "GetModuleHandleW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    //private static partial IntPtr GetModuleHandleCore(string lpModuleName);

    //[LibraryImport("kernel32", EntryPoint = "GetProcAddress", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    //private static partial IntPtr GetProcAddressCore(IntPtr hModule, string procName);

    //[LibraryImport("kernel32", EntryPoint = "IsWow64Process", SetLastError = true)]
    //[return: MarshalAs(UnmanagedType.Bool)]
    //private static partial bool IsWow64ProcessCore(IntPtr hProcess, [MarshalAs(UnmanagedType.Bool)] out bool lpSystemInfo);

    public static HANDLE GetCurrentProcess() => PInvoke.GetCurrentProcess();

    //[LibraryImport("kernel32", EntryPoint = "FormatMessageW", SetLastError = true)]
    //private static partial uint FormatMessage(uint dwFlags, IntPtr lpSource, uint dwMessageId, uint dwLanguageId, IntPtr lpBuffer, int nSize, IntPtr Arguments);
        
    //const uint FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;

    [Flags]
    public enum DesiredAccess : uint
    {
        None,
        Write = 0x40000000,
        Read = 0x80000000
    }

    public static HMODULE GetModuleHandle(string moduleName)
    {
        var hModule = PInvoke.GetModuleHandle(moduleName);
        if (hModule == HMODULE.Null) throw new Win32ErrorException();

        return hModule;
    }

    public static FARPROC GetProcAddress(HMODULE hModule, string procName)
    {
        var farProc = PInvoke.GetProcAddress(hModule, procName);
        if (farProc == FARPROC.Null) throw new Win32ErrorException();

        return farProc;
    }

    public static bool IsWow64Process(HANDLE hProcess)
    {
        if (!PInvoke.IsWow64Process(hProcess, out var result)) throw new Win32ErrorException();
            
        return result;
    }

    public static HANDLE CreateFile(
        string fileName,
        FILE_SHARE_MODE shareMode,
        FILE_CREATION_DISPOSITION creationDisposition,
        DesiredAccess desiredAccess = DesiredAccess.None,
        SECURITY_ATTRIBUTES? securityAttributes = null,
        FILE_FLAGS_AND_ATTRIBUTES flagsAndAttributes = 0,
        HANDLE templateFile = default)
    {
        var handle = PInvoke.CreateFile(fileName, (uint)desiredAccess, shareMode, securityAttributes, creationDisposition, flagsAndAttributes, templateFile);
        if (handle == HANDLE.INVALID_HANDLE_VALUE) throw new Win32ErrorException();

        return handle;
    }

    public static bool TryCreateFile(
        string fileName,
        FILE_SHARE_MODE shareMode,
        FILE_CREATION_DISPOSITION creationDisposition,
        out HANDLE handle,
        DesiredAccess desiredAccess = DesiredAccess.None,
        SECURITY_ATTRIBUTES? securityAttributes = null,
        FILE_FLAGS_AND_ATTRIBUTES flagsAndAttributes = 0,
        HANDLE templateFile = default)
    {
        handle = PInvoke.CreateFile(fileName, (uint)desiredAccess, shareMode, securityAttributes, creationDisposition, flagsAndAttributes, templateFile);

        return handle != HANDLE.INVALID_HANDLE_VALUE;
    }

    public static unsafe string FormatMessage(int errorCode)
    {
        const int capacity = 255;
        Span<char> message = stackalloc char[capacity];

        var charsWritten = PInvoke.FormatMessage(FORMAT_MESSAGE_OPTIONS.FORMAT_MESSAGE_FROM_SYSTEM, null, (uint)errorCode, 0, message, capacity, null);
        if (charsWritten == 0) throw new Win32ErrorException();
        return message.Slice(0, (int)charsWritten).ToString();
    }
}