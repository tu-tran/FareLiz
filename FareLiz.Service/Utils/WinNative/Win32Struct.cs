using System.Runtime.InteropServices;

namespace SkyDean.FareLiz.Service.Utils.WinNative
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }
}
