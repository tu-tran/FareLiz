using System;

namespace SkyDean.FareLiz.Service.Utils.WinNative
{
    [Flags]
    internal enum ProcessAccessFlags : uint
    {
        Read = 0x10, // PROCESS_VM_READ
        QueryInformation = 0x400 // PROCESS_QUERY_INFORMATION
    }
}
