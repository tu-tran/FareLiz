namespace SkyDean.FareLiz.WinForm.Components.Controls.TabControl
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// The sub class.
    /// </summary>
    internal class SubClass : NativeWindow
    {
        /// <summary>
        /// The sub class wnd proc event handler.
        /// </summary>
        /// <param name="m">
        /// The m.
        /// </param>
        public delegate int SubClassWndProcEventHandler(ref Message m);

        /// <summary>
        /// The is sub classed.
        /// </summary>
        private bool IsSubClassed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubClass"/> class.
        /// </summary>
        /// <param name="Handle">
        /// The handle.
        /// </param>
        /// <param name="_SubClass">
        /// The _ sub class.
        /// </param>
        public SubClass(IntPtr Handle, bool _SubClass)
        {
            this.AssignHandle(Handle);
            this.IsSubClassed = _SubClass;
        }

        /// <summary>
        /// Gets or sets a value indicating whether sub classed.
        /// </summary>
        public bool SubClassed
        {
            get
            {
                return this.IsSubClassed;
            }

            set
            {
                this.IsSubClassed = value;
            }
        }

        /// <summary>
        /// The sub classed wnd proc.
        /// </summary>
        public event SubClassWndProcEventHandler SubClassedWndProc;

        /// <summary>
        /// The wnd proc.
        /// </summary>
        /// <param name="m">
        /// The m.
        /// </param>
        protected override void WndProc(ref Message m)
        {
            if (this.IsSubClassed)
            {
                if (this.OnSubClassedWndProc(ref m) != 0)
                {
                    return;
                }
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// The call default wnd proc.
        /// </summary>
        /// <param name="m">
        /// The m.
        /// </param>
        public void CallDefaultWndProc(ref Message m)
        {
            base.WndProc(ref m);
        }

        /// <summary>
        /// The on sub classed wnd proc.
        /// </summary>
        /// <param name="m">
        /// The m.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int OnSubClassedWndProc(ref Message m)
        {
            if (this.SubClassedWndProc != null)
            {
                return this.SubClassedWndProc(ref m);
            }

            return 0;
        }

        /// <summary>
        /// The hi word.
        /// </summary>
        /// <param name="Number">
        /// The number.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int HiWord(int Number)
        {
            return (Number >> 16) & 0xffff;
        }

        /// <summary>
        /// The lo word.
        /// </summary>
        /// <param name="Number">
        /// The number.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int LoWord(int Number)
        {
            return Number & 0xffff;
        }

        /// <summary>
        /// The make long.
        /// </summary>
        /// <param name="LoWord">
        /// The lo word.
        /// </param>
        /// <param name="HiWord">
        /// The hi word.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int MakeLong(int LoWord, int HiWord)
        {
            return (HiWord << 16) | (LoWord & 0xffff);
        }

        /// <summary>
        /// The make l param.
        /// </summary>
        /// <param name="LoWord">
        /// The lo word.
        /// </param>
        /// <param name="HiWord">
        /// The hi word.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        public IntPtr MakeLParam(int LoWord, int HiWord)
        {
            return (IntPtr)((HiWord << 16) | (LoWord & 0xffff));
        }
    }
}