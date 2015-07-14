namespace SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.DatePicker
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    using SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.Helper;
    using SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.Interfaces;

    /// <summary>Control that handles displaying and entering a date.</summary>
    partial class DatePickerDateTextBox
    {
        /// <summary>TextBox control that handles the input of a date.</summary>
        internal class InputDateTextBox : TextBox
        {
            #region Fields

            /// <summary>The _parent.</summary>
            private readonly DatePickerDateTextBox _parent;

            #endregion

            #region constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="InputDateTextBox"/> class.
            /// </summary>
            /// <param name="parent">
            /// The _parent of the control.
            /// </param>
            public InputDateTextBox(DatePickerDateTextBox parent)
            {
                if (parent == null)
                {
                    throw new ArgumentNullException("parent");
                }

                this._parent = parent;
                this.BorderStyle = BorderStyle.None;
                this.BackColor = parent.BackColor;
            }

            #endregion

            #region Properties

            /// <summary>Gets or sets the back color of the text box.</summary>
            public override sealed Color BackColor
            {
                get
                {
                    return base.BackColor;
                }

                set
                {
                    base.BackColor = value;
                }
            }

            #endregion

            #region events

            /// <summary>Event that is raised if the user input has ended.</summary>
            public event EventHandler FinishedEditing;

            #endregion

            #region methods

            /// <summary>
            /// If the corresponding <see cref="DatePickerDateTextBox" /> uses native digits converts the current input to a string with arabic numerals,
            /// otherwise returns <see cref="InputDateTextBox.Text" />.
            /// </summary>
            /// <returns>The current input string with arabic numerals.</returns>
            public string GetCurrentText()
            {
                var input = this.Text;

                if (string.IsNullOrEmpty(input))
                {
                    return string.Empty;
                }

                if (this._parent._enhancedDatePicker.UseNativeDigits)
                {
                    return input.ToList().ConvertAll(this.GetArabicNumeralString).Aggregate((s1, s2) => s1 + s2);
                }

                return input;
            }

            /// <summary>
            /// Processes a dialog key.
            /// </summary>
            /// <param name="keyData">
            /// A <see cref="Keys"/> value that represents the key to process.
            /// </param>
            /// <returns>
            /// true if the key was processed by the control; otherwise, false.
            /// </returns>
            protected override bool ProcessDialogKey(Keys keyData)
            {
                if (keyData == Keys.Enter || keyData == Keys.Tab || keyData == Keys.Escape)
                {
                    if (keyData == Keys.Escape)
                    {
                        this.Text = string.Empty;
                    }

                    this.RaiseFinishedEditing();

                    return true;
                }

                return base.ProcessDialogKey(keyData);
            }

            /// <summary>
            /// Raises the <see cref="Control.KeyDown"/> event.
            /// </summary>
            /// <param name="e">
            /// A <see cref="KeyEventArgs"/> that contains the event data.
            /// </param>
            protected override void OnKeyDown(KeyEventArgs e)
            {
                if (this._parent._enhancedDatePicker.AllowPromptAsInput)
                {
                    base.OnKeyDown(e);

                    return;
                }

                e.Handled = true;

                if ((e.KeyCode < Keys.D0 || e.KeyCode > Keys.D9) && (e.KeyCode < Keys.NumPad0 || e.KeyCode > Keys.NumPad9))
                {
                    switch (e.KeyCode)
                    {
                        case Keys.Back:
                        case Keys.Left:
                        case Keys.Right:
                        case Keys.Home:
                        case Keys.End:
                            {
                                e.Handled = false;

                                break;
                            }

                        default:
                            {
                                e.SuppressKeyPress = true;

                                break;
                            }
                    }
                }

                base.OnKeyDown(e);
            }

            /// <summary>
            /// Raises the <see cref="Control.KeyPress"/> event.
            /// </summary>
            /// <param name="e">
            /// A <see cref="KeyPressEventArgs"/> that contains the event data.
            /// </param>
            protected override void OnKeyPress(KeyPressEventArgs e)
            {
                var isNumber = char.IsNumber(e.KeyChar);
                var isSeparator = this._parent._enhancedDatePicker.FormatProvider.DateSeparator.Contains(e.KeyChar);

                var textContainsSeparator = this.Text.Contains(this._parent._enhancedDatePicker.FormatProvider.DateSeparator);
                var txtLength = textContainsSeparator ? 10 : 8;

                if (isSeparator)
                {
                    if (this.Text.Length == txtLength && e.KeyChar != '\b')
                    {
                        e.Handled = true;
                    }
                    else
                    {
                        base.OnKeyPress(e);
                    }

                    return;
                }

                if ((!isNumber || this.Text.Length == txtLength) && e.KeyChar != '\b')
                {
                    e.Handled = true;

                    return;
                }

                if (this._parent._enhancedDatePicker.UseNativeDigits && isNumber)
                {
                    var number = int.Parse(e.KeyChar.ToString(CultureInfo.InvariantCulture));

                    var nativeNumber = DateMethods.GetNativeNumberString(
                        number, 
                        this._parent._enhancedDatePicker.Culture.NumberFormat.NativeDigits, 
                        false);

                    e.KeyChar = nativeNumber[0];
                }

                base.OnKeyPress(e);
            }

            /// <summary>
            /// Raises the <see cref="System.Windows.Forms.Control.LostFocus"/> event.
            /// </summary>
            /// <param name="e">
            /// An <see cref="System.EventArgs"/> that contains the event data.
            /// </param>
            protected override void OnLostFocus(EventArgs e)
            {
                base.OnLostFocus(e);

                this.RaiseFinishedEditing();
            }

            /// <summary>Raises the <see cref="FinishedEditing" /> event.</summary>
            private void RaiseFinishedEditing()
            {
                var handler = this.FinishedEditing;

                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }

            /// <summary>
            /// Returns for the native digits represented by <paramref name="nativeDigit"/> the arabic numeral string representation.
            /// </summary>
            /// <param name="nativeDigit">
            /// The native digit.
            /// </param>
            /// <returns>
            /// The arabic numeral string representation for the native digit specified by <paramref name="nativeDigit"/>.
            /// </returns>
            private string GetArabicNumeralString(char nativeDigit)
            {
                var nativeDigits = this._parent._enhancedDatePicker.Culture.NumberFormat.NativeDigits;

                for (var i = 0; i < 10; i++)
                {
                    if (nativeDigit == nativeDigits[i][0])
                    {
                        return i.ToString(CultureInfo.InvariantCulture);
                    }
                }

                return nativeDigit.ToString(CultureInfo.CurrentUICulture);
            }

            #endregion
        }

        /// <summary>Class that parses a date _pattern and stores _pattern specific information.</summary>
        private class DatePatternParser
        {
            #region constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="DatePatternParser"/> class.
            /// </summary>
            /// <param name="pattern">
            /// The date _pattern.
            /// </param>
            /// <param name="provider">
            /// The format _provider.
            /// </param>
            /// <exception cref="ArgumentNullException">
            /// If <paramref name="provider"/> is <c>null</c>.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// If <paramref name="pattern"/> is <c>null</c> or empty.
            /// </exception>
            public DatePatternParser(string pattern, ICustomFormatProvider provider)
            {
                if (string.IsNullOrEmpty(pattern))
                {
                    throw new InvalidOperationException("parameter '_pattern' cannot be null or empty.");
                }

                if (provider == null)
                {
                    throw new ArgumentNullException("provider", "parameter '_provider' cannot be null.");
                }

                this._provider = provider;
                this._pattern = pattern;
            }

            #endregion

            #region Fields

            /// <summary>The _provider.</summary>
            private readonly ICustomFormatProvider _provider;

            /// <summary>The _pattern.</summary>
            private readonly string _pattern = string.Empty;

            /// <summary>The day string.</summary>
            private string dayString = string.Empty;

            /// <summary>The day name string.</summary>
            private string dayNameString = string.Empty;

            /// <summary>The month string.</summary>
            private string monthString = string.Empty;

            /// <summary>The year string.</summary>
            private string yearString = string.Empty;

            /// <summary>The era string.</summary>
            private string eraString = string.Empty;

            /// <summary>The day part index.</summary>
            private int dayPartIndex = -1;

            /// <summary>The month part index.</summary>
            private int monthPartIndex = -1;

            /// <summary>The year part index.</summary>
            private int yearPartIndex = -1;

            /// <summary>The day index.</summary>
            private int dayIndex = -1;

            /// <summary>The month index.</summary>
            private int monthIndex = -1;

            /// <summary>The year index.</summary>
            private int yearIndex = -1;

            /// <summary>The is day number.</summary>
            private bool isDayNumber;

            /// <summary>The is month number.</summary>
            private bool isMonthNumber;

            #endregion

            #region  properties

            /// <summary>Gets the day string.</summary>
            public string DayString
            {
                get
                {
                    return this.dayString;
                }
            }

            /// <summary>Gets the day name string.</summary>
            public string DayNameString
            {
                get
                {
                    return this.dayNameString;
                }
            }

            /// <summary>Gets the month string.</summary>
            public string MonthString
            {
                get
                {
                    return this.monthString;
                }
            }

            /// <summary>Gets the year string.</summary>
            public string YearString
            {
                get
                {
                    return this.yearString;
                }
            }

            /// <summary>Gets the era string.</summary>
            public string EraString
            {
                get
                {
                    return this.eraString;
                }
            }

            /// <summary>Gets a value indicating whether is day number.</summary>
            public bool IsDayNumber
            {
                get
                {
                    return this.isDayNumber;
                }
            }

            /// <summary>Gets a value indicating whether is month number.</summary>
            public bool IsMonthNumber
            {
                get
                {
                    return this.isMonthNumber;
                }
            }

            /// <summary>Gets the day part index.</summary>
            public int DayPartIndex
            {
                get
                {
                    return this.dayPartIndex;
                }
            }

            /// <summary>Gets the month part index.</summary>
            public int MonthPartIndex
            {
                get
                {
                    return this.monthPartIndex;
                }
            }

            /// <summary>Gets the year part index.</summary>
            public int YearPartIndex
            {
                get
                {
                    return this.yearPartIndex;
                }
            }

            /// <summary>Gets the day index.</summary>
            public int DayIndex
            {
                get
                {
                    return this.dayIndex;
                }
            }

            /// <summary>Gets the month index.</summary>
            public int MonthIndex
            {
                get
                {
                    return this.monthIndex;
                }
            }

            /// <summary>Gets the year index.</summary>
            public int YearIndex
            {
                get
                {
                    return this.yearIndex;
                }
            }

            #endregion

            #region methods

            /// <summary>
            /// The parse pattern.
            /// </summary>
            /// <param name="date">
            /// The date.
            /// </param>
            /// <param name="nativeDigits">
            /// The native digits.
            /// </param>
            /// <returns>
            /// The <see cref="string"/>.
            /// </returns>
            public string ParsePattern(MonthCalendarDate date, string[] nativeDigits = null)
            {
                // replace date separator with '/'
                var format = this._pattern.Replace(this._provider.DateSeparator, "/");

                var sb = new StringBuilder();

                var c = this._provider.Calendar;

                var i = 0;
                var index = 0;

                while (i < format.Length)
                {
                    int tokLen;
                    var ch = format[i];
                    string currentString;

                    switch (ch)
                    {
                        case 'd':
                            {
                                tokLen = CountChar(format, i, ch);

                                if (tokLen <= 2)
                                {
                                    currentString = DateMethods.GetNumberString(date.Day, nativeDigits, tokLen == 2);

                                    this.isDayNumber = true;

                                    this.dayString = currentString;

                                    this.dayPartIndex = index++;

                                    this.dayIndex = sb.Length;
                                }
                                else
                                {
                                    currentString = tokLen == 3
                                                        ? this._provider.GetAbbreviatedDayName(c.GetDayOfWeek(date.Date))
                                                        : this._provider.GetDayName(c.GetDayOfWeek(date.Date));

                                    this.dayNameString = currentString;
                                }

                                sb.Append(currentString);

                                break;
                            }

                        case 'M':
                            {
                                tokLen = CountChar(format, i, ch);

                                if (tokLen <= 2)
                                {
                                    currentString = DateMethods.GetNumberString(date.Month, nativeDigits, tokLen == 2);

                                    this.isMonthNumber = true;
                                }
                                else
                                {
                                    currentString = tokLen == 3
                                                        ? this._provider.GetAbbreviatedMonthName(date.Year, date.Month)
                                                        : this._provider.GetMonthName(date.Year, date.Month);
                                }

                                this.monthPartIndex = index++;

                                this.monthIndex = sb.Length;

                                this.monthString = currentString;

                                sb.Append(currentString);

                                break;
                            }

                        case 'y':
                            {
                                tokLen = CountChar(format, i, ch);

                                var year = tokLen <= 2 ? date.Year % 100 : date.Year;

                                currentString = DateMethods.GetNumberString(year, nativeDigits, tokLen <= 2);

                                this.yearString = currentString;

                                this.yearPartIndex = index++;

                                this.yearIndex = sb.Length;

                                sb.Append(currentString);

                                break;
                            }

                        case 'g':
                            {
                                tokLen = CountChar(format, i, ch);

                                currentString = this._provider.GetEraName(c.GetEra(date.Date));

                                this.eraString = currentString;

                                sb.Append(currentString);

                                break;
                            }

                        case '/':
                            {
                                tokLen = CountChar(format, i, ch);

                                sb.Append(this._provider.DateSeparator);

                                break;
                            }

                        default:
                            {
                                tokLen = 1;

                                sb.Append(ch.ToString(CultureInfo.CurrentUICulture));

                                break;
                            }
                    }

                    i += tokLen;
                }

                return sb.ToString();
            }

            /// <summary>
            /// Counts the specified <paramref name="c"/> at the position specified by <paramref name="p"/> in the string specified by
            /// <paramref name="fmt"/>.
            /// </summary>
            /// <param name="fmt">
            /// The string value to search.
            /// </param>
            /// <param name="p">
            /// The position start at.
            /// </param>
            /// <param name="c">
            /// The char value to count.
            /// </param>
            /// <returns>
            /// The count of the char <paramref name="c"/> at the specified location.
            /// </returns>
            private static int CountChar(string fmt, int p, char c)
            {
                var l = fmt.Length;
                var i = p + 1;

                while ((i < l) && (fmt[i] == c))
                {
                    i++;
                }

                return i - p;
            }

            #endregion
        }
    }
}