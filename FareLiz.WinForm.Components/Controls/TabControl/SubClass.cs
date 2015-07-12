namespace SkyDean.FareLiz.WinForm.Components.Controls.TabControl
{
    using System;
    using System.Windows.Forms;

    internal class SubClass : NativeWindow
    {
        public delegate int SubClassWndProcEventHandler(ref Message m);

        private bool IsSubClassed = false;

        public SubClass(IntPtr Handle, bool _SubClass)
        {
            base.AssignHandle(Handle);
            this.IsSubClassed = _SubClass;
        }

        public bool SubClassed
        {
            get { return this.IsSubClassed; }
            set { this.IsSubClassed = value; }
        }

        public event SubClassWndProcEventHandler SubClassedWndProc;

        protected override void WndProc(ref Message m)
        {
            if (this.IsSubClassed)
            {
                if (this.OnSubClassedWndProc(ref m) != 0)
                    return;
            }
            base.WndProc(ref m);
        }

        public void CallDefaultWndProc(ref Message m)
        {
            base.WndProc(ref m);
        }

        private int OnSubClassedWndProc(ref Message m)
        {
            if (this.SubClassedWndProc != null)
            {
                return this.SubClassedWndProc(ref m);
            }

            return 0;
        }

        public int HiWord(int Number)
        {
            return ((Number >> 16) & 0xffff);
        }

        public int LoWord(int Number)
        {
            return (Number & 0xffff);
        }

        public int MakeLong(int LoWord, int HiWord)
        {
            return (HiWord << 16) | (LoWord & 0xffff);
        }

        public IntPtr MakeLParam(int LoWord, int HiWord)
        {
            return (IntPtr)((HiWord << 16) | (LoWord & 0xffff));
        }
    }
}
