namespace SkyDean.FareLiz.WinForm.Components.Dialog
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    using SkyDean.FareLiz.WinForm.Components.Utils;

    /*  Modified from FlexibleMessageBox – A flexible replacement for the .NET MessageBox
     * 
     *  Author:         Jörg Reichert (public@jreichert.de)
     *  Version:        1.1
     *  Published at:   http://www.codeproject.com/Articles/601900/FlexibleMessageBox
     *  
     */

    /// <summary>
    /// The ex message box.
    /// </summary>
    public partial class ExMessageBox : SmartForm
    {
        /// <summary>
        /// The button type.
        /// </summary>
        public enum ButtonType
        {
            /// <summary>
            /// The ok.
            /// </summary>
            OK = 0, 

            /// <summary>
            /// The cancel.
            /// </summary>
            Cancel, 

            /// <summary>
            /// The yes.
            /// </summary>
            Yes, 

            /// <summary>
            /// The no.
            /// </summary>
            No, 

            /// <summary>
            /// The abort.
            /// </summary>
            Abort, 

            /// <summary>
            /// The retry.
            /// </summary>
            Retry, 

            /// <summary>
            /// The ignore.
            /// </summary>
            Ignore
        };

        /// <summary>
        /// The max width ratio.
        /// </summary>
        public static double MaxWidthRatio = 0.7;

        /// <summary>
        /// The max height ratio.
        /// </summary>
        public static double MaxHeightRatio = 0.9;

        /// <summary>
        /// The dialog font.
        /// </summary>
        public static Font DialogFont = SystemFonts.MessageBoxFont;

        /// <summary>
        /// The _button text.
        /// </summary>
        private readonly Dictionary<ButtonType, string> _buttonText = new Dictionary<ButtonType, string>
                                                                          {
                                                                              { ButtonType.OK, "OK" }, 
                                                                              { ButtonType.Cancel, "Cancel" }, 
                                                                              { ButtonType.Yes, "Yes" }, 
                                                                              { ButtonType.No, "No" }, 
                                                                              { ButtonType.Abort, "Abort" }, 
                                                                              { ButtonType.Retry, "Retry" }, 
                                                                              { ButtonType.Ignore, "Ignore" }
                                                                          };

        /// <summary>
        /// The _default button.
        /// </summary>
        private MessageBoxDefaultButton _defaultButton;

        /// <summary>
        /// The _visible buttons count.
        /// </summary>
        private int _visibleButtonsCount;

        /// <summary>
        /// Prevents a default instance of the <see cref="ExMessageBox"/> class from being created. Initializes a new instance of the <see cref="FlexibleMessageBoxForm"/> class.
        /// </summary>
        private ExMessageBox()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the caption text.
        /// </summary>
        public string CaptionText { get; set; }

        /// <summary>The text that is been used in the FlexibleMessageBoxForm.</summary>
        public string MessageText { get; set; }

        /// <summary>
        /// Gets the string rows.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="string[]"/>.
        /// </returns>
        private static string[] GetStringRows(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return null;
            }

            var messageRows = message.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.None);
            return messageRows;
        }

        /// <summary>
        /// Gets the button text
        /// </summary>
        /// <param name="buttonType">
        /// The button Type.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetButtonText(ButtonType buttonType)
        {
            return this._buttonText[buttonType];
        }

        /// <summary>
        /// Ensure the given working area factor in the range of  0.2 - 1.0 where: 0.2 means:  Half as large as the working area. 1.0 means:  As large as the
        /// working area.
        /// </summary>
        /// <param name="workingAreaFactor">
        /// The given working area factor.
        /// </param>
        /// <returns>
        /// The corrected given working area factor.
        /// </returns>
        private static double GetCorrectedWorkingAreaFactor(double workingAreaFactor)
        {
            const double MIN_FACTOR = 0.2;
            const double MAX_FACTOR = 1.0;

            if (workingAreaFactor < MIN_FACTOR)
            {
                return MIN_FACTOR;
            }

            if (workingAreaFactor > MAX_FACTOR)
            {
                return MAX_FACTOR;
            }

            return workingAreaFactor;
        }

        /// <summary>
        /// Set the dialogs start position when given. Otherwise center the dialog on the current screen.
        /// </summary>
        /// <param name="flexibleMessageBoxForm">
        /// The FlexibleMessageBox dialog.
        /// </param>
        /// <param name="owner">
        /// The owner.
        /// </param>
        private static void SetDialogStartPosition(ExMessageBox flexibleMessageBoxForm, IWin32Window owner)
        {
            // If no owner given: Center on current screen
            if (owner == null)
            {
                var screen = Screen.FromPoint(Cursor.Position);
                flexibleMessageBoxForm.StartPosition = FormStartPosition.Manual;
                flexibleMessageBoxForm.Left = screen.Bounds.Left + screen.Bounds.Width / 2 - flexibleMessageBoxForm.Width / 2;
                flexibleMessageBoxForm.Top = screen.Bounds.Top + screen.Bounds.Height / 2 - flexibleMessageBoxForm.Height / 2;
            }
        }

        /// <summary>
        /// Calculate the dialogs start size (Try to auto-size width to show longest text row). Also set the maximum dialog size.
        /// </summary>
        /// <param name="flexibleMessageBoxForm">
        /// The FlexibleMessageBox dialog.
        /// </param>
        /// <param name="text">
        /// The text (the longest text row is used to calculate the dialog width).
        /// </param>
        private static void SetDialogSizes(ExMessageBox flexibleMessageBoxForm, string text)
        {
            // Set maximum dialog size
            var maxFormSize = new Size(
                Convert.ToInt32(SystemInformation.WorkingArea.Width * GetCorrectedWorkingAreaFactor(MaxWidthRatio)), 
                Convert.ToInt32(SystemInformation.WorkingArea.Height * GetCorrectedWorkingAreaFactor(MaxHeightRatio)));
            var borderSize = SystemInformation.BorderSize;
            var borderWidth = 2 * borderSize.Width;
            var borderHeight = 2 * borderSize.Height;
            var contentPadding = flexibleMessageBoxForm.contentPanel.Padding;
            var contentPaddingX = contentPadding.Left + contentPadding.Right;
            var contentPaddingY = contentPadding.Top + contentPadding.Bottom;
            var maxTxtSize = new Size(maxFormSize.Width - borderWidth - contentPaddingX, maxFormSize.Height - borderHeight - contentPaddingY);

            // Calculate dialog start size: Try to auto-size width to show longest text row
            var stringSize = TextRenderer.MeasureText(
                text, 
                flexibleMessageBoxForm.richTextBoxMessage.Font, 
                maxTxtSize, 
                TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);

            // Set dialog start size
            var mainContentHeight = stringSize.Height + contentPaddingY;
            var mainContentWidth = stringSize.Width + contentPaddingX;
            var formWidth = mainContentWidth + borderWidth;
            var formHeight = mainContentHeight + flexibleMessageBoxForm.buttonPanel.Height + borderHeight;

            var icon = flexibleMessageBoxForm.pictureBoxForIcon;
            if (icon.Image == null)
            {
                icon.Padding = Padding.Empty;
            }
            else
            {
                icon.Padding = new Padding(0, 0, 10, 0);
                formWidth += icon.Width;
            }

            flexibleMessageBoxForm.ClientSize = new Size(formWidth, formHeight);
            if (flexibleMessageBoxForm.Width > maxFormSize.Width)
            {
                flexibleMessageBoxForm.Width = maxFormSize.Width;
            }

            if (flexibleMessageBoxForm.Height > maxFormSize.Height)
            {
                flexibleMessageBoxForm.Height = maxFormSize.Height;
            }

            flexibleMessageBoxForm.MaximumSize = flexibleMessageBoxForm.Size;
        }

        /// <summary>
        /// Set the dialogs icon. When no icon is used: Correct placement and width of rich text box.
        /// </summary>
        /// <param name="flexibleMessageBoxForm">
        /// The FlexibleMessageBox dialog.
        /// </param>
        /// <param name="icon">
        /// The MessageBoxIcon.
        /// </param>
        private static void SetDialogIcon(ExMessageBox flexibleMessageBoxForm, MessageBoxIcon icon)
        {
            Icon targetIcon = null;
            switch (icon)
            {
                case MessageBoxIcon.Information:
                    targetIcon = SystemIcons.Information;
                    break;
                case MessageBoxIcon.Warning:
                    targetIcon = SystemIcons.Warning;
                    break;
                case MessageBoxIcon.Error:
                    targetIcon = SystemIcons.Error;
                    break;
                case MessageBoxIcon.Question:
                    targetIcon = SystemIcons.Question;
                    break;
                default:

                    // When no icon is used: Correct placement and width of rich text box.
                    flexibleMessageBoxForm.pictureBoxForIcon.Visible = false;
                    flexibleMessageBoxForm.richTextBoxMessage.Left -= flexibleMessageBoxForm.pictureBoxForIcon.Width;
                    flexibleMessageBoxForm.richTextBoxMessage.Width += flexibleMessageBoxForm.pictureBoxForIcon.Width;
                    break;
            }

            flexibleMessageBoxForm.pictureBoxForIcon.Image = targetIcon.ToBitmap();
        }

        /// <summary>
        /// Set dialog buttons visibilities and texts. Also set a default button.
        /// </summary>
        /// <param name="flexibleMessageBoxForm">
        /// The FlexibleMessageBox dialog.
        /// </param>
        /// <param name="buttons">
        /// The buttons.
        /// </param>
        /// <param name="defaultButton">
        /// The default button.
        /// </param>
        private static void SetDialogButtons(ExMessageBox flexibleMessageBoxForm, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        {
            // Set the buttons visibilities and texts
            switch (buttons)
            {
                case MessageBoxButtons.AbortRetryIgnore:
                    flexibleMessageBoxForm._visibleButtonsCount = 3;

                    flexibleMessageBoxForm.button1.Visible = true;
                    flexibleMessageBoxForm.button1.Text = flexibleMessageBoxForm.GetButtonText(ButtonType.Abort);
                    flexibleMessageBoxForm.button1.DialogResult = DialogResult.Abort;

                    flexibleMessageBoxForm.button2.Visible = true;
                    flexibleMessageBoxForm.button2.Text = flexibleMessageBoxForm.GetButtonText(ButtonType.Retry);
                    flexibleMessageBoxForm.button2.DialogResult = DialogResult.Retry;

                    flexibleMessageBoxForm.button3.Visible = true;
                    flexibleMessageBoxForm.button3.Text = flexibleMessageBoxForm.GetButtonText(ButtonType.Ignore);
                    flexibleMessageBoxForm.button3.DialogResult = DialogResult.Ignore;
                    break;

                case MessageBoxButtons.OKCancel:
                    flexibleMessageBoxForm._visibleButtonsCount = 2;

                    flexibleMessageBoxForm.button2.Visible = true;
                    flexibleMessageBoxForm.button2.Text = flexibleMessageBoxForm.GetButtonText(ButtonType.OK);
                    flexibleMessageBoxForm.button2.DialogResult = DialogResult.OK;

                    flexibleMessageBoxForm.button3.Visible = true;
                    flexibleMessageBoxForm.button3.Text = flexibleMessageBoxForm.GetButtonText(ButtonType.Cancel);
                    flexibleMessageBoxForm.button3.DialogResult = DialogResult.Cancel;
                    break;

                case MessageBoxButtons.RetryCancel:
                    flexibleMessageBoxForm._visibleButtonsCount = 2;

                    flexibleMessageBoxForm.button2.Visible = true;
                    flexibleMessageBoxForm.button2.Text = flexibleMessageBoxForm.GetButtonText(ButtonType.Retry);
                    flexibleMessageBoxForm.button2.DialogResult = DialogResult.Retry;

                    flexibleMessageBoxForm.button3.Visible = true;
                    flexibleMessageBoxForm.button3.Text = flexibleMessageBoxForm.GetButtonText(ButtonType.Cancel);
                    flexibleMessageBoxForm.button3.DialogResult = DialogResult.Cancel;
                    break;

                case MessageBoxButtons.YesNo:
                    flexibleMessageBoxForm._visibleButtonsCount = 2;

                    flexibleMessageBoxForm.button2.Visible = true;
                    flexibleMessageBoxForm.button2.Text = flexibleMessageBoxForm.GetButtonText(ButtonType.Yes);
                    flexibleMessageBoxForm.button2.DialogResult = DialogResult.Yes;

                    flexibleMessageBoxForm.button3.Visible = true;
                    flexibleMessageBoxForm.button3.Text = flexibleMessageBoxForm.GetButtonText(ButtonType.No);
                    flexibleMessageBoxForm.button3.DialogResult = DialogResult.No;
                    break;

                case MessageBoxButtons.YesNoCancel:
                    flexibleMessageBoxForm._visibleButtonsCount = 3;

                    flexibleMessageBoxForm.button1.Visible = true;
                    flexibleMessageBoxForm.button1.Text = flexibleMessageBoxForm.GetButtonText(ButtonType.Yes);
                    flexibleMessageBoxForm.button1.DialogResult = DialogResult.Yes;

                    flexibleMessageBoxForm.button2.Visible = true;
                    flexibleMessageBoxForm.button2.Text = flexibleMessageBoxForm.GetButtonText(ButtonType.No);
                    flexibleMessageBoxForm.button2.DialogResult = DialogResult.No;

                    flexibleMessageBoxForm.button3.Visible = true;
                    flexibleMessageBoxForm.button3.Text = flexibleMessageBoxForm.GetButtonText(ButtonType.Cancel);
                    flexibleMessageBoxForm.button3.DialogResult = DialogResult.Cancel;
                    break;

                case MessageBoxButtons.OK:
                default:
                    flexibleMessageBoxForm._visibleButtonsCount = 1;
                    flexibleMessageBoxForm.button3.Visible = true;
                    flexibleMessageBoxForm.button3.Text = flexibleMessageBoxForm.GetButtonText(ButtonType.OK);
                    flexibleMessageBoxForm.button3.DialogResult = DialogResult.OK;
                    break;
            }

            // Set default button (used in FlexibleMessageBoxForm_Shown)
            flexibleMessageBoxForm._defaultButton = defaultButton;
        }

        /// <summary>
        /// The on shown.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            int buttonIndexToFocus = 1;
            Button buttonToFocus;

            // Set the default button...
            switch (this._defaultButton)
            {
                case MessageBoxDefaultButton.Button1:
                default:
                    buttonIndexToFocus = 1;
                    break;
                case MessageBoxDefaultButton.Button2:
                    buttonIndexToFocus = 2;
                    break;
                case MessageBoxDefaultButton.Button3:
                    buttonIndexToFocus = 3;
                    break;
            }

            if (buttonIndexToFocus > this._visibleButtonsCount)
            {
                buttonIndexToFocus = this._visibleButtonsCount;
            }

            if (buttonIndexToFocus == 3)
            {
                buttonToFocus = this.button3;
            }
            else if (buttonIndexToFocus == 2)
            {
                buttonToFocus = this.button2;
            }
            else
            {
                buttonToFocus = this.button1;
            }

            buttonToFocus.Focus();
        }

        /// <summary>
        /// Handles the LinkClicked event of the richTextBoxMessage control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.Forms.LinkClickedEventArgs"/> instance containing the event data.
        /// </param>
        private void richTextBoxMessage_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                BrowserUtils.Open(e.LinkText);
            }
            catch
            {
            }
 // Ignore the error
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        /// <summary>
        /// The set translation.
        /// </summary>
        /// <param name="translation">
        /// The translation.
        /// </param>
        private void SetTranslation(Dictionary<ButtonType, string> translation)
        {
            if (translation == null || translation.Count < 1)
            {
                return;
            }

            foreach (var k in translation.Keys)
            {
                this._buttonText[k] = translation[k];
            }
        }

        /// <summary>
        /// The show.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="DialogResult"/>.
        /// </returns>
        public static DialogResult Show(string text)
        {
            return Show(null, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// The show.
        /// </summary>
        /// <param name="owner">
        /// The owner.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="DialogResult"/>.
        /// </returns>
        public static DialogResult Show(IWin32Window owner, string text)
        {
            return Show(owner, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// The show.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="caption">
        /// The caption.
        /// </param>
        /// <returns>
        /// The <see cref="DialogResult"/>.
        /// </returns>
        public static DialogResult Show(string text, string caption)
        {
            return Show(null, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// The show.
        /// </summary>
        /// <param name="owner">
        /// The owner.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="caption">
        /// The caption.
        /// </param>
        /// <returns>
        /// The <see cref="DialogResult"/>.
        /// </returns>
        public static DialogResult Show(IWin32Window owner, string text, string caption)
        {
            return Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// The show.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="caption">
        /// The caption.
        /// </param>
        /// <param name="buttons">
        /// The buttons.
        /// </param>
        /// <returns>
        /// The <see cref="DialogResult"/>.
        /// </returns>
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons)
        {
            return Show(null, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// The show.
        /// </summary>
        /// <param name="owner">
        /// The owner.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="caption">
        /// The caption.
        /// </param>
        /// <param name="buttons">
        /// The buttons.
        /// </param>
        /// <returns>
        /// The <see cref="DialogResult"/>.
        /// </returns>
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons)
        {
            return Show(owner, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// The show.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="caption">
        /// The caption.
        /// </param>
        /// <param name="buttons">
        /// The buttons.
        /// </param>
        /// <param name="icon">
        /// The icon.
        /// </param>
        /// <returns>
        /// The <see cref="DialogResult"/>.
        /// </returns>
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return Show(null, text, caption, buttons, icon, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// The show.
        /// </summary>
        /// <param name="owner">
        /// The owner.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="caption">
        /// The caption.
        /// </param>
        /// <param name="buttons">
        /// The buttons.
        /// </param>
        /// <param name="icon">
        /// The icon.
        /// </param>
        /// <returns>
        /// The <see cref="DialogResult"/>.
        /// </returns>
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return Show(owner, text, caption, buttons, icon, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// The show.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="caption">
        /// The caption.
        /// </param>
        /// <param name="buttons">
        /// The buttons.
        /// </param>
        /// <param name="icon">
        /// The icon.
        /// </param>
        /// <param name="defaultButton">
        /// The default button.
        /// </param>
        /// <returns>
        /// The <see cref="DialogResult"/>.
        /// </returns>
        public static DialogResult Show(
            string text, 
            string caption, 
            MessageBoxButtons buttons, 
            MessageBoxIcon icon, 
            MessageBoxDefaultButton defaultButton)
        {
            return Show(null, text, caption, buttons, icon, defaultButton);
        }

        /// <summary>
        /// The show.
        /// </summary>
        /// <param name="owner">
        /// The owner.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="caption">
        /// The caption.
        /// </param>
        /// <param name="buttons">
        /// The buttons.
        /// </param>
        /// <param name="icon">
        /// The icon.
        /// </param>
        /// <param name="defaultButton">
        /// The default button.
        /// </param>
        /// <returns>
        /// The <see cref="DialogResult"/>.
        /// </returns>
        public static DialogResult Show(
            IWin32Window owner, 
            string text, 
            string caption, 
            MessageBoxButtons buttons, 
            MessageBoxIcon icon, 
            MessageBoxDefaultButton defaultButton)
        {
            return Show(null, text, caption, buttons, icon, defaultButton, null);
        }

        /// <summary>
        /// The show.
        /// </summary>
        /// <param name="owner">
        /// The owner.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="caption">
        /// The caption.
        /// </param>
        /// <param name="buttons">
        /// The buttons.
        /// </param>
        /// <param name="icon">
        /// The icon.
        /// </param>
        /// <param name="defaultButton">
        /// The default button.
        /// </param>
        /// <param name="translation">
        /// The translation.
        /// </param>
        /// <returns>
        /// The <see cref="DialogResult"/>.
        /// </returns>
        public static DialogResult Show(
            IWin32Window owner, 
            string text, 
            string caption, 
            MessageBoxButtons buttons, 
            MessageBoxIcon icon, 
            MessageBoxDefaultButton defaultButton, 
            Dictionary<ButtonType, string> translation)
        {
            // Create a new instance of the FlexibleMessageBox form
            using (var flexibleMessageBoxForm = new ExMessageBox())
            {
                // Set the font for all controls
                flexibleMessageBoxForm.Font = DialogFont;

                // Bind the caption and the message text
                flexibleMessageBoxForm.CaptionText = caption;
                flexibleMessageBoxForm.MessageText = text;
                flexibleMessageBoxForm.FlexibleMessageBoxFormBindingSource.DataSource = flexibleMessageBoxForm;

                if (translation != null && translation.Count > 0)
                {
                    flexibleMessageBoxForm.SetTranslation(translation);
                }

                // Set the buttons visibilities and texts. Also set a default button.
                SetDialogButtons(flexibleMessageBoxForm, buttons, defaultButton);

                // Set the dialogs icon. When no icon is used: Correct placement and width of rich text box.
                SetDialogIcon(flexibleMessageBoxForm, icon);

                flexibleMessageBoxForm.richTextBoxMessage.Font = DialogFont;

                // Calculate the dialogs start size (Try to auto-size width to show longest text row). Also set the maximum dialog size. 
                SetDialogSizes(flexibleMessageBoxForm, text);

                // Set the dialogs start position when given. Otherwise center the dialog on the current screen.
                SetDialogStartPosition(flexibleMessageBoxForm, owner);

                // Show the dialog
                return flexibleMessageBoxForm.ShowDialog(owner);
            }
        }
    }
}