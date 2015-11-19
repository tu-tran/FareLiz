namespace SkyDean.FareLiz.Service.Utils.WinNative
{
    using System;

    /// <summary>
    /// The process access flags.
    /// </summary>
    [Flags]
    internal enum ProcessAccessFlags : uint
    {
        /// <summary>
        /// The read.
        /// </summary>
        Read = 0x10, // PROCESS_VM_READ

        /// <summary>
        /// The query information.
        /// </summary>
        QueryInformation = 0x400 // PROCESS_QUERY_INFORMATION
    }
}