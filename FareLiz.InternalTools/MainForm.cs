namespace FareLiz.InternalTools
{
    #region

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Windows.Forms;

    using SkyDean.FareLiz.Core.Utils;
    using SkyDean.FareLiz.WinForm.Components.Dialog;

    #endregion

    /// <summary>The main form.</summary>
    public partial class MainForm : SmartForm
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="MainForm" /> class.</summary>
        public MainForm()
        {
            this.InitializeComponent();
            this.InitializeGreps();
        }

        #endregion

        #region Fields

        /// <summary>The _active grep.</summary>
        private DataGrep _activeGrep;

        /// <summary>The _is changing.</summary>
        private bool _isChanging;

        #endregion

        #region Methods

        /// <summary>
        /// The generate bytes string.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GenerateBytesString(string input)
        {
            var bytes = this._activeGrep.Convert(input);

            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                sb.AppendFormat("0x{0:x2}, ", b);
            }

            var result = sb.ToString().TrimEnd(", ".ToCharArray());
            return result;
        }

        /// <summary>
        /// The generate data.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        private void GenerateData(object sender)
        {
            if (sender == this.txtRaw)
            {
                this.txtEncoded.Text = this.GenerateBytesString(this.txtRaw.Text);
            }
            else if (sender == this.txtEncoded)
            {
                if (string.IsNullOrEmpty(this.txtEncoded.Text))
                {
                    this.txtRaw.Text = null;
                }
                else
                {
                    try
                    {
                        var arr = this.txtEncoded.Text.Split(',');
                        var bytesArr = new List<byte>();
                        foreach (var item in arr)
                        {
                            var trimmedItem = item.Trim();
                            if (trimmedItem.Length > 2)
                            {
                                var bytePart = trimmedItem.Substring(2);
                                bytesArr.Add(Convert.ToByte(bytePart, 16));
                            }
                        }

                        this.txtRaw.Text = this._activeGrep.Convert(bytesArr.ToArray());
                    }
                    catch
                    {
                        this.txtRaw.Text = "<Invalid Hex>";
                    }
                }
            }
        }

        /// <summary>The initialize greps.</summary>
        private void InitializeGreps()
        {
            var curLoc = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var files = Directory.GetFiles(curLoc, "*.dll");
            var greps = new List<FieldInfo>();
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic;

            foreach (var fi in files)
            {
                try
                {
                    var asm = Assembly.LoadFile(fi);
                    var types = asm.GetTypes();
                    foreach (var t in types)
                    {
                        var fields = t.GetFieldsRecursively(flags);
                        foreach (var f in fields)
                        {
                            if (f.FieldType == typeof(DataGrep))
                            {
                                var defConstructor = t.GetConstructor(Type.EmptyTypes);
                                if (defConstructor != null)
                                {
                                    greps.Add(f);
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }

            this.cbGrep.DisplayMember = "DeclaringType";
            this.cbGrep.DataSource = greps;
        }

        /// <summary>
        /// The text box_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            var txt = (TextBox)sender;
            if (e.Control && e.KeyCode == Keys.A)
            {
                txt.SelectAll();
            }
        }

        /// <summary>
        /// The text box_ text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            if (this._isChanging)
            {
                return;
            }

            this._isChanging = true;
            this.GenerateData(sender);
            this._isChanging = false;
        }

        /// <summary>
        /// The cb grep_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void cbGrep_SelectedIndexChanged(object sender, EventArgs e)
        {
            var f = this.cbGrep.SelectedItem as FieldInfo;
            if (f != null)
            {
                var type = f.DeclaringType;
                if (this._activeGrep == null || this._activeGrep.GetType() != type)
                {
                    var decInstance = Activator.CreateInstance(type);
                    this._activeGrep = f.GetValue(decInstance) as DataGrep;
                }

                this.GenerateData(sender);
            }
        }

        #endregion
    }
}