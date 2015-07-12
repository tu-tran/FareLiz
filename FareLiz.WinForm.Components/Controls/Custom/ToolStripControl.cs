namespace SkyDean.FareLiz.WinForm.Components.Controls.Custom
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;
    using System.Windows.Forms.Design;

    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.StatusStrip)]
    public class ToolStripControlAgent : ToolStripControlHost
    {
        public ToolStripControlAgent()
            : base(null)
        {
        }

        public ToolStripControlAgent(Control c) : base(c)
        {
        }
    }

    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.StatusStrip)]
    public class ToolStripControl<T> : ToolStripControlAgent where T : Control
    {
        public ToolStripControl()
            : base(Activator.CreateInstance<T>())
        {
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public T ControlItem
        {
            get { return (T)this.Control; }
        }
    }
}
