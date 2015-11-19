namespace SkyDean.FareLiz.WinForm.Components.Controls.Custom
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;
    using System.Windows.Forms.Design;

    /// <summary>
    /// The tool strip control agent.
    /// </summary>
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.StatusStrip)]
    public class ToolStripControlAgent : ToolStripControlHost
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolStripControlAgent"/> class.
        /// </summary>
        public ToolStripControlAgent()
            : base(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolStripControlAgent"/> class.
        /// </summary>
        /// <param name="c">
        /// The c.
        /// </param>
        public ToolStripControlAgent(Control c)
            : base(c)
        {
        }
    }

    /// <summary>
    /// The tool strip control.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.StatusStrip)]
    public class ToolStripControl<T> : ToolStripControlAgent
        where T : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolStripControl{T}"/> class.
        /// </summary>
        public ToolStripControl()
            : base(Activator.CreateInstance<T>())
        {
        }

        /// <summary>
        /// Gets the control item.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public T ControlItem
        {
            get
            {
                return (T)this.Control;
            }
        }
    }
}