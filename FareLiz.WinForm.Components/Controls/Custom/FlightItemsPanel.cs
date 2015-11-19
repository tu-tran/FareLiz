namespace SkyDean.FareLiz.WinForm.Components.Controls.Custom
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Globalization;
    using System.Windows.Forms;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Core.Utils;
    using SkyDean.FareLiz.WinForm.Components.Properties;
    using SkyDean.FareLiz.WinForm.Components.Utils;

    /// <summary>
    /// The flight items panel.
    /// </summary>
    public partial class FlightItemsPanel : UserControl
    {
        /// <summary>
        /// The _border pen.
        /// </summary>
        private readonly Pen _borderPen = new Pen(Brushes.DarkSlateGray) { DashStyle = DashStyle.Dot };

        /// <summary>
        /// The _dec price back color.
        /// </summary>
        private readonly Color _decPriceBackColor = Color.FromArgb(241, 255, 242);

        /// <summary>
        /// The _dec price brush.
        /// </summary>
        private readonly Brush _decPriceBrush = Brushes.LimeGreen;

        /// <summary>
        /// The _inc price back color.
        /// </summary>
        private readonly Color _incPriceBackColor = Color.FromArgb(255, 245, 242);

        /// <summary>
        /// The _inc price brush.
        /// </summary>
        private readonly Brush _incPriceBrush = Brushes.DeepPink;

        /// <summary>
        /// The _new price back color.
        /// </summary>
        private readonly Color _newPriceBackColor = Color.FromArgb(255, 255, 242);

        /// <summary>
        /// The _new price brush.
        /// </summary>
        private readonly Brush _newPriceBrush = Brushes.DeepSkyBlue;

        /// <summary>
        /// The _number culture.
        /// </summary>
        private readonly CultureInfo _numberCulture = new CultureInfo(CultureInfo.CurrentCulture.Name)
                                                          {
                                                              NumberFormat =
                                                                  new NumberFormatInfo
                                                                      {
                                                                          NumberDecimalSeparator
                                                                              = ".", 
                                                                          NumberGroupSeparator
                                                                              = ","
                                                                      }
                                                          };

        /// <summary>
        /// The _price font.
        /// </summary>
        private readonly Font _priceFont;

        /// <summary>
        /// The _hover row.
        /// </summary>
        private int _hoverRow = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightItemsPanel"/> class.
        /// </summary>
        public FlightItemsPanel()
        {
            this.InitializeComponent();
            this.colFirstPrice.DefaultCellStyle.Font =
                this.colSecondPrice.DefaultCellStyle.Font = this._priceFont = new Font(this.Font, FontStyle.Bold);
            this.gvFlightItems.BackgroundColor = this.BackColor;
            this.gvFlightItems.CreateControl();
        }

        /// <summary>
        /// Gets the expected height.
        /// </summary>
        public int ExpectedHeight
        {
            get
            {
                int labelHeight = this.lblHeader.Height + this.lblHeader.Padding.Top + this.lblHeader.Padding.Bottom;
                int flightHeight = this.gvFlightItems.Rows.GetRowsHeight(DataGridViewElementStates.Visible);
                return labelHeight + flightHeight;
            }
        }

        /// <summary>
        /// The auto resize.
        /// </summary>
        /// <param name="maxHeight">
        /// The max height.
        /// </param>
        /// <returns>
        /// The <see cref="Size"/>.
        /// </returns>
        internal Size AutoResize(int maxHeight)
        {
            this.gvFlightItems.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
            this.colIcon.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            this.colIcon.Width = this.colIcon.MinimumWidth;

            int targetHeight = this.ExpectedHeight;
            int borderWidth = 2 * SystemInformation.BorderSize.Width;

            int flightWidth = this.gvFlightItems.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) + borderWidth;
            if (targetHeight >= maxHeight)
            {
                flightWidth += SystemInformation.VerticalScrollBarWidth;
            }

            int titleWidth = this.lblHeader.Width + this.lblHeader.Padding.Left + this.lblHeader.Padding.Right;

            int targetWidth = titleWidth > flightWidth ? titleWidth : flightWidth;

            var result = new Size(targetWidth, targetHeight);
            this.colIcon.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            return result;
        }

        /// <summary>
        /// The bind.
        /// </summary>
        /// <param name="header">
        /// The header.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        public void Bind(string header, IList<FlightMonitorItem> data)
        {
            this.lblHeader.Text = header;
            this.lblHeader.AutoSize = !string.IsNullOrEmpty(header);
            if (!this.lblHeader.AutoSize)
            {
                this.lblHeader.Size = Size.Empty;
            }

            this.gvFlightItems.SuspendLayout();

            try
            {
                this.gvFlightItems.Rows.Clear();
                DataGridViewRow[] rows = this.GetDataRows(data);
                if (rows != null)
                {
                    this.gvFlightItems.Rows.AddRange(rows);
                }
            }
            finally
            {
                this.gvFlightItems.ResumeLayout();
            }
        }

        /// <summary>
        /// The get data rows.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="DataGridViewRow[]"/>.
        /// </returns>
        private DataGridViewRow[] GetDataRows(IList<FlightMonitorItem> data)
        {
            int dataCount = data == null ? 0 : data.Count;
            if (dataCount == 0)
            {
                return null;
            }

            var currency = data[0].FlightData.JourneyData.Currency;
            var currencySymbol = AppContext.MonitorEnvironment.CurrencyProvider.GetCurrencyInfo(currency).Symbol;
            var result = new DataGridViewRow[data.Count];
            string decimalSep = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;

            for (int i = 0; i < dataCount; i++)
            {
                var item = data[i];
                var flightData = item.FlightData;

                var iconCell = new DataGridViewImageCell();
                var opCell = new DataGridViewTextBoxCell { Value = flightData.Operator };
                if (flightData.CanBePurchased)
                {
                    opCell.Style.ForeColor = Color.SteelBlue;
                }

                var durationCell = new DataGridViewTextBoxCell { Value = flightData.Duration.ToHourMinuteString() };

                double firstPrice = item.FlightStatus == FlightStatus.New ? flightData.Price : item.OldPrice;
                double roundedFirstPrice = Math.Round(firstPrice);
                var firstPriceCell = new DataGridViewTextBoxCell { Value = roundedFirstPrice };
                var firstPriceCurrencyCell = new DataGridViewTextBoxCell { Value = currencySymbol };

                var newRow = new DataGridViewRow { Tag = item };
                newRow.Cells.AddRange(iconCell, opCell, durationCell, firstPriceCell, firstPriceCurrencyCell);

                if (item.FlightStatus == FlightStatus.New)
                {
                    newRow.DefaultCellStyle.BackColor = this._newPriceBackColor;
                }
                else
                {
                    newRow.DefaultCellStyle.BackColor = item.FlightStatus == FlightStatus.PriceDecreased
                                                             ? this._decPriceBackColor
                                                             : this._incPriceBackColor;

                    // Price status
                    var img = item.FlightStatus == FlightStatus.PriceDecreased ? Resources.DownArrow : Resources.UpArrow;
                    var statusCell = new DataGridViewImageCell { Value = img };

                    // New price
                    double roundedSecondPrice = Math.Round(flightData.Price);
                    var secondPriceCell = new DataGridViewTextBoxCell { Value = roundedSecondPrice };
                    var secondPriceCurrencyCell = new DataGridViewTextBoxCell { Value = currencySymbol };

                    newRow.Cells.AddRange(statusCell, secondPriceCell, secondPriceCurrencyCell);
                }

                result[i] = newRow;
            }

            return result;
        }

        /// <summary>
        /// The gv flight items_ cell painting.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void gvFlightItems_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            var rect = e.CellBounds;
            if (rect.Bottom < 0 || rect.Right < 0)
            {
                return;
            }

            e.PaintBackground(rect, true);

            var cellColumn = this.gvFlightItems.Columns[e.ColumnIndex];

            // Draw off-target decimal part for the prices
            if (e.FormattedValue != null)
            {
                if (cellColumn == this.colFirstPrice || cellColumn == this.colSecondPriceCurrency)
                {
                    string val = e.FormattedValue.ToString();
                    if (val.Length > 0)
                    {
                        var font = e.CellStyle.Font;
                        var txtSize = TextRenderer.MeasureText(val, font);
                        int txtX = rect.X + (cellColumn == this.colFirstPrice ? 4 : -2);
                        int txtY = rect.Y + ((rect.Height - txtSize.Height) / 2) + 1;
                        e.Graphics.DrawString(val, e.CellStyle.Font, SystemBrushes.ControlText, txtX, txtY);
                    }
                }
                else
                {
                    e.PaintContent(rect);
                }
            }

            rect.Inflate(0, -1 * (int)this._borderPen.Width);

            var y = rect.Y + rect.Height; // Draw dotted bottom border
            e.Graphics.DrawLine(this._borderPen, rect.X, y, rect.Right, y);
            if (e.RowIndex == 0)
            {
                e.Graphics.DrawLine(this._borderPen, rect.X, rect.Y, rect.Right, rect.Y);
            }

            using (Pen edgePen = (cellColumn == this.colIcon) ? new Pen(ControlPaint.Dark(e.CellStyle.BackColor), 3) : null)
            {
                // Draw left border for all cells except for Operator cell and price section
                if (cellColumn == this.colIcon || cellColumn == this.colDuration || cellColumn == this.colFirstPrice)
                {
                    e.Graphics.DrawLine(edgePen ?? this._borderPen, rect.X, rect.Y, rect.X, rect.Bottom);
                }
                else if (cellColumn == this.colSecondPriceCurrency)
                {
                    // Draw right border for the last column
                    e.Graphics.DrawLine(this._borderPen, rect.Right - 1, rect.Y, rect.Right - 1, rect.Bottom);
                }
            }

            if (e.ColumnIndex == 0)
            {
                // Draw arrow on the first column (this is a walk-around for the tooltip)
                var flightItem = this.gvFlightItems.Rows[e.RowIndex].Tag as FlightMonitorItem;
                if (flightItem != null)
                {
                    int edgeSize = rect.Height / 3;
                    var status = flightItem.FlightStatus;
                    Brush brush = status == FlightStatus.PriceIncreased
                                       ? this._incPriceBrush
                                       : (status == FlightStatus.PriceDecreased ? this._decPriceBrush : this._newPriceBrush);

                    if (this._hoverRow == e.RowIndex)
                    {
                        int arrowSize = rect.Height / 2;
                        int arrowSpan = (int)(arrowSize * 1.2 / 2);

                        int arrowXLeft = rect.Left - 1;
                        int arrowXMiddle = rect.Left + arrowSize - 1;

                        int arrowYTop = rect.Top + 2;
                        int arrowYMiddle = arrowYTop + arrowSpan;
                        int arrowYBottom = arrowYMiddle + arrowSpan;

                        var arrow = new[]
                                        {
                                            new Point(arrowXLeft, arrowYTop), new Point(arrowXMiddle, arrowYMiddle), new Point(arrowXLeft, arrowYBottom)
                                        };

                        using (var arrowBrush = new SolidBrush(this.toolTip.BackColor))
                        {
                            e.Graphics.FillPolygon(arrowBrush, arrow);
                        }
                    }
                }
            }

            e.Handled = true;
        }

        /// <summary>
        /// The gv flight items_ cell double click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void gvFlightItems_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var flightItem = this.gvFlightItems.Rows[e.RowIndex].Tag as FlightMonitorItem;
            if (flightItem != null)
            {
                var data = flightItem.FlightData;
                if (data != null && data.CanBePurchased)
                {
                    try
                    {
                        BrowserUtils.Open(data.TravelAgency.Url);
                    }
                    catch (Exception ex)
                    {
                        string error = "Failed to launch web browser for ticket purchase: " + ex.Message;
                        AppContext.Logger.Error(error);
                        MessageBox.Show(error, "Ticket Purchase", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }

        /// <summary>
        /// The gv flight items_ cell mouse enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void gvFlightItems_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            var flightItem = this.gvFlightItems.Rows[e.RowIndex].Tag as FlightMonitorItem;
            if (flightItem != null)
            {
                var data = flightItem.FlightData;
                DataGridViewRow row = this.gvFlightItems.Rows[e.RowIndex];
                bool canPurchase = data != null && data.CanBePurchased;

                if (canPurchase)
                {
                    row.Cells[0].Value = Resources.Purchase;
                    this.Cursor = Cursors.Hand;
                }
                else
                {
                    row.Cells[0].Value = null;
                    this.Cursor = Cursors.Default;
                }

                if (this._hoverRow != e.RowIndex)
                {
                    var rowRect = this.gvFlightItems.GetRowDisplayRectangle(e.RowIndex, true);

                    this._hoverRow = e.RowIndex;
                    string caption = (canPurchase ? "Double-click to buy this ticket:" : "This ticket cannot be bought at the moment!")
                                     + Environment.NewLine + data.SummaryString;

                    var rowScreenRect = this.gvFlightItems.RectangleToScreen(rowRect);
                    var relRowRect = this.lblHeader.RectangleToClient(rowScreenRect);
                    Point rowMiddleLeft = new Point(relRowRect.X, relRowRect.Y + (relRowRect.Height / 2));

                    // Show the tooltip on the edge of the panel
                    this.toolTip.Show(caption, this, rowMiddleLeft);
                }
            }
        }

        /// <summary>
        /// The gv flight items_ cell mouse leave.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void gvFlightItems_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            this.gvFlightItems.Rows[e.RowIndex].Cells[0].Value = null;
            this.Cursor = Cursors.Default;

            if (this._hoverRow == e.RowIndex)
            {
                var mouseLoc = this.gvFlightItems.PointToClient(MousePosition);
                var rowRect = this.gvFlightItems.GetRowDisplayRectangle(this._hoverRow, true);
                if (!rowRect.Contains(mouseLoc))
                {
                    this.toolTip.Hide(this.lblHeader); // Hide tooltip if the mouse has changed to another row
                    this._hoverRow = -1;
                }
            }
        }

        /// <summary>
        /// The gv flight items_ selection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void gvFlightItems_SelectionChanged(object sender, EventArgs e)
        {
            this.gvFlightItems.ClearSelection();
        }

        /// <summary>
        /// The gv flight items_ mouse enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void gvFlightItems_MouseEnter(object sender, EventArgs e)
        {
            this.gvFlightItems.Focus();
        }

        /// <summary>
        /// The gv flight items_ mouse leave.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void gvFlightItems_MouseLeave(object sender, EventArgs e)
        {
            var mousePos = MousePosition;
            var mouseLoc = this.gvFlightItems.PointToClient(mousePos);
            if (!this.gvFlightItems.Bounds.Contains(mouseLoc))
            {
                // The tooltip may have stolen the focus, thus this check is needed
                if (this._hoverRow > -1)
                {
                    this.gvFlightItems.InvalidateRow(this._hoverRow);
                    this._hoverRow = -1;
                }

                this.toolTip.Hide(this.lblHeader);
            }
        }
    }
}