namespace SkyDean.FareLiz.Service.Utils.WinNative
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// The rect.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        /// <summary>
        /// The left.
        /// </summary>
        public int left;

        /// <summary>
        /// The top.
        /// </summary>
        public int top;

        /// <summary>
        /// The right.
        /// </summary>
        public int right;

        /// <summary>
        /// The bottom.
        /// </summary>
        public int bottom;
    }
}