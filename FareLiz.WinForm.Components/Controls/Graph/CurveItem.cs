//============================================================================
//ZedGraph Class Library - A Flexible Line Graph/Bar Graph Library in C#
//Copyright � 2004  John Champion
//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 2.1 of the License, or (at your option) any later version.
//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//Lesser General Public License for more details.
//You should have received a copy of the GNU Lesser General Public
//License along with this library; if not, write to the Free Software
//Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//=============================================================================

#if ( !DOTNET1 ) // Is this a .Net 2 compilation?

#endif

namespace SkyDean.FareLiz.WinForm.Components.Controls.Graph
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    /// <summary>
    /// This class contains the data and methods for an individual curve within a graph pane.  It carries the settings for the curve including the key and
    /// item names, colors, symbols and sizes, linetypes, etc.
    /// </summary>
    /// <author> John Champion modified by Jerry Vos </author>
    /// <version> $Revision: 3.43 $ $Date: 2007-11-03 04:41:28 $ </version>
    [Serializable]
    public abstract class CurveItem : ISerializable, ICloneable
    {
        #region Fields

        /// <summary>
        /// protected field that stores a <see cref="Label" /> instance for this
        /// <see cref="CurveItem" />, which is used for the <see cref="Legend" />
        /// label.  Use the public property <see cref="Label" /> to access this value.
        /// </summary>
        internal Label _label;

        /// <summary>
        /// protected field that stores the boolean value that determines whether this
        /// <see cref="CurveItem" /> is on the bottom X axis or the top X axis (X2). Use the public property <see cref="IsX2Axis" /> to access this value.
        /// </summary>
        protected bool _isX2Axis;

        /// <summary>
        /// protected field that stores the boolean value that determines whether this
        /// <see cref="CurveItem" /> is on the left Y axis or the right Y axis (Y2). Use the public property <see cref="IsY2Axis" /> to access this value.
        /// </summary>
        protected bool _isY2Axis;

        /// <summary>
        /// protected field that stores the index number of the Y Axis to which this
        /// <see cref="CurveItem" /> belongs.  Use the public property <see cref="YAxisIndex" />
        /// to access this value.
        /// </summary>
        protected int _yAxisIndex;

        /// <summary>
        /// protected field that stores the boolean value that determines whether this
        /// <see cref="CurveItem" /> is visible on the graph. Use the public property <see cref="IsVisible" /> to access this value. Note that this value turns
        /// the curve display on or off, but it does not affect the display of the legend entry.  To hide the legend entry, you have to set
        /// <see cref="Graph.Label.IsVisible" /> to false.
        /// </summary>
        protected bool _isVisible;

        // Revision: JCarpenter 10/06
        /// <summary>
        /// Protected field that stores the boolean value that determines whether this
        /// <see cref="CurveItem" /> is selected on the graph. Use the public property <see cref="IsSelected" /> to access this value. Note that this value
        /// changes the curve display color, but it does not affect the display of the legend entry.  To hide the legend entry, you have to set
        /// <see cref="Graph.Label.IsVisible" /> to false.
        /// </summary>
        protected bool _isSelected;

        // Revision: JCarpenter 10/06
        /// <summary>Protected field that stores the boolean value that determines whether this
        /// <see cref="CurveItem" /> can be selected in the graph.</summary>
        protected bool _isSelectable;

        /// <summary>
        /// protected field that stores a boolean value which allows you to override the normal ordinal axis behavior.  Use the public property
        /// <see cref="IsOverrideOrdinal" /> to access this value.
        /// </summary>
        protected bool _isOverrideOrdinal;

        /// <summary>
        /// The <see cref="IPointList" /> of value sets that represent this <see cref="CurveItem" />. The size of this list determines the number of points that
        /// are plotted.  Note that values defined as System.Double.MaxValue are considered "missing" values (see <see cref="PointPairBase.Missing" />), and are
        /// not plotted.  The curve will have a break at these points to indicate the values are missing.
        /// </summary>
        protected IPointList _points;

        /// <summary>
        /// A tag object for use by the user.  This can be used to store additional information associated with the <see cref="CurveItem" />.  ZedGraph does not
        /// use this value for any purpose.
        /// </summary>
        /// <remarks>
        /// Note that, if you are going to Serialize ZedGraph data, then any type that you store in <see cref="Tag" /> must be a serializable type (or it will
        /// cause an exception).
        /// </remarks>
        public object Tag;

        /// <summary>Protected field that stores the hyperlink information for this object.</summary>
        internal Link _link;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CurveItem"/> class. 
        /// <see cref="CurveItem"/> constructor the pre-specifies the curve label, the x and y data values as a <see cref="IPointList"/>, the curve type (Bar
        /// or Line/Symbol), the <see cref="Color"/>, and the
        /// <see cref="SymbolType"/>. Other properties of the curve are defaulted to the values in the <see cref="GraphPane.Default"/> class.
        /// </summary>
        /// <param name="label">
        /// A string label (legend entry) for this curve
        /// </param>
        /// <param name="x">
        /// An array of double precision values that define the independent (X axis) values for this curve
        /// </param>
        /// <param name="y">
        /// An array of double precision values that define the dependent (Y axis) values for this curve
        /// </param>
        public CurveItem(string label, double[] x, double[] y)
            : this(label, new PointPairList(x, y))
        {
        }

        /*	
		public CurveItem( string _label, int  y ) : this(  _label, new IPointList( ) )
		{
		}
*/

        /// <summary>
        /// Initializes a new instance of the <see cref="CurveItem"/> class. 
        /// <see cref="CurveItem"/> constructor the pre-specifies the curve label, the x and y data values as a <see cref="IPointList"/>, the curve type (Bar
        /// or Line/Symbol), the <see cref="Color"/>, and the
        /// <see cref="SymbolType"/>. Other properties of the curve are defaulted to the values in the <see cref="GraphPane.Default"/> class.
        /// </summary>
        /// <param name="label">
        /// A string label (legend entry) for this curve
        /// </param>
        /// <param name="points">
        /// A <see cref="IPointList"/> of double precision value pairs that define the X and Y values for this curve
        /// </param>
        public CurveItem(string label, IPointList points)
        {
            this.Init(label);

            if (points == null)
            {
                this._points = new PointPairList();
            }
            else
            {
                // this.points = (IPointList) _points.Clone();
                this._points = points;
            }
        }

        /// <summary>
        /// Internal initialization routine thats sets some initial values to defaults.
        /// </summary>
        /// <param name="label">
        /// A string label (legend entry) for this curve
        /// </param>
        private void Init(string label)
        {
            this._label = new Label(label, null);
            this._isY2Axis = false;
            this._isX2Axis = false;
            this._isVisible = true;
            this._isOverrideOrdinal = false;
            this.Tag = null;
            this._yAxisIndex = 0;
            this._link = new Link();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurveItem"/> class. 
        /// <see cref="CurveItem"/> constructor that specifies the label of the CurveItem. This is the same as <c>CurveItem(label, null, null)</c>.
        /// <seealso cref="CurveItem( string, double[], double[] )"/>
        /// </summary>
        /// <param name="label">
        /// A string label (legend entry) for this curve
        /// </param>
        public CurveItem(string label)
            : this(label, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurveItem"/> class. 
        /// </summary>
        public CurveItem()
        {
            this.Init(null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurveItem"/> class. 
        /// The Copy Constructor
        /// </summary>
        /// <param name="rhs">
        /// The CurveItem object from which to copy
        /// </param>
        public CurveItem(CurveItem rhs)
        {
            this._label = rhs._label.Clone();
            this._isY2Axis = rhs.IsY2Axis;
            this._isX2Axis = rhs.IsX2Axis;
            this._isVisible = rhs.IsVisible;
            this._isOverrideOrdinal = rhs._isOverrideOrdinal;
            this._yAxisIndex = rhs._yAxisIndex;

            if (rhs.Tag is ICloneable)
            {
                this.Tag = ((ICloneable)rhs.Tag).Clone();
            }
            else
            {
                this.Tag = rhs.Tag;
            }

            this._points = (IPointList)rhs.Points.Clone();

            this._link = rhs._link.Clone();
        }

        /// <summary>Implement the <see cref="ICloneable" /> interface in a typesafe manner by just calling the typed version of Clone.</summary>
        /// <remarks>
        /// Note that this method must be called with an explicit cast to ICloneable, and that it is inherently virtual.  For example:
        /// <code>
        /// ParentClass foo = new ChildClass();
        /// ChildClass bar = (ChildClass) ((ICloneable)foo).Clone();
        /// </code>
        /// Assume that ChildClass is inherited from ParentClass.  Even though foo is declared with ParentClass, it is actually an instance of ChildClass.
        /// Calling the ICloneable implementation of Clone() on foo actually calls ChildClass.Clone() as if it were a virtual function.
        /// </remarks>
        /// <returns>A deep copy of this object</returns>
        object ICloneable.Clone()
        {
            throw new NotImplementedException("Can't clone an abstract base type -- child types must implement ICloneable");

            // return new PaneBase( this );
        }

        #endregion

        #region Serialization

        /// <summary>Current schema value that defines the version of the serialized file</summary>
        public const int schema = 11;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurveItem"/> class. 
        /// Constructor for deserializing objects
        /// </summary>
        /// <param name="info">
        /// A <see cref="SerializationInfo"/> instance that defines the serialized data
        /// </param>
        /// <param name="context">
        /// A <see cref="StreamingContext"/> instance that contains the serialized data
        /// </param>
        protected CurveItem(SerializationInfo info, StreamingContext context)
        {
            // The schema value is just a file version parameter.  You can use it to make future versions
            // backwards compatible as new member variables are added to classes
            int sch = info.GetInt32("schema");

            this._label = (Label)info.GetValue("label", typeof(Label));
            this._isY2Axis = info.GetBoolean("isY2Axis");
            if (sch >= 11)
            {
                this._isX2Axis = info.GetBoolean("isX2Axis");
            }
            else
            {
                this._isX2Axis = false;
            }

            this._isVisible = info.GetBoolean("isVisible");

            this._isOverrideOrdinal = info.GetBoolean("isOverrideOrdinal");

            // Data Points are always stored as a PointPairList, regardless of the
            // actual original type (which could be anything that supports IPointList).
            this._points = (PointPairList)info.GetValue("points", typeof(PointPairList));

            this.Tag = info.GetValue("Tag", typeof(object));

            this._yAxisIndex = info.GetInt32("yAxisIndex");

            this._link = (Link)info.GetValue("link", typeof(Link));
        }

        /// <summary>
        /// Populates a <see cref="SerializationInfo"/> instance with the data needed to serialize the target object
        /// </summary>
        /// <param name="info">
        /// A <see cref="SerializationInfo"/> instance that defines the serialized data
        /// </param>
        /// <param name="context">
        /// A <see cref="StreamingContext"/> instance that contains the serialized data
        /// </param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("schema", schema);
            info.AddValue("label", this._label);
            info.AddValue("isY2Axis", this._isY2Axis);
            info.AddValue("isX2Axis", this._isX2Axis);
            info.AddValue("isVisible", this._isVisible);
            info.AddValue("isOverrideOrdinal", this._isOverrideOrdinal);

            // if points is already a PointPairList, use it
            // otherwise, create a new PointPairList so it can be serialized
            PointPairList list;
            if (this._points is PointPairList)
            {
                list = this._points as PointPairList;
            }
            else
            {
                list = new PointPairList(this._points);
            }

            info.AddValue("points", list);
            info.AddValue("Tag", this.Tag);
            info.AddValue("yAxisIndex", this._yAxisIndex);

            info.AddValue("link", this._link);
        }

        #endregion

        #region Properties

        /// <summary>A <see cref="Label" /> instance that represents the <see cref="Legend" />
        /// entry for the this <see cref="CurveItem" /> object</summary>
        public Label Label
        {
            get
            {
                return this._label;
            }

            set
            {
                this._label = value;
            }
        }

        /// <summary>
        /// The <see cref="Line" />/<see cref="Symbol" />/<see cref="Bar" />
        /// color (FillColor for the Bar).  This is a common access to
        /// <see cref="LineBase.Color">Line.Color</see>,
        /// <see cref="LineBase.Color">Border.Color</see>, and
        /// <see cref="Fill.Color">Fill.Color</see> properties for this curve.
        /// </summary>
        public Color Color
        {
            get
            {
                if (this is BarItem)
                {
                    return ((BarItem)this).Bar.Fill.Color;
                }

                if (this is LineItem && ((LineItem)this).Line.IsVisible)
                {
                    return ((LineItem)this).Line.Color;
                }

                if (this is LineItem)
                {
                    return ((LineItem)this).Symbol.Border.Color;
                }

                if (this is ErrorBarItem)
                {
                    return ((ErrorBarItem)this).Bar.Color;
                }

                if (this is HiLowBarItem)
                {
                    return ((HiLowBarItem)this).Bar.Fill.Color;
                }

                return Color.Empty;
            }

            set
            {
                if (this is BarItem)
                {
                    ((BarItem)this).Bar.Fill.Color = value;
                }
                else if (this is LineItem)
                {
                    ((LineItem)this).Line.Color = value;
                    ((LineItem)this).Symbol.Border.Color = value;
                    ((LineItem)this).Symbol.Fill.Color = value;
                }
                else if (this is ErrorBarItem)
                {
                    ((ErrorBarItem)this).Bar.Color = value;
                }
                else if (this is HiLowBarItem)
                {
                    ((HiLowBarItem)this).Bar.Fill.Color = value;
                }
            }
        }

        /// <summary>
        /// Determines whether this <see cref="CurveItem" /> is visible on the graph. Note that this value turns the curve display on or off, but it does not
        /// affect the display of the legend entry.  To hide the legend entry, you have to set <see cref="Graph.Label.IsVisible" /> to false.
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return this._isVisible;
            }

            set
            {
                this._isVisible = value;
            }
        }

        // Revision: JCarpenter 10/06
        /// <summary>
        /// Determines whether this <see cref="CurveItem" /> is selected on the graph. Note that this value changes the curve displayed color, but it does not
        /// affect the display of the legend entry. To hide the legend entry, you have to set <see cref="Graph.Label.IsVisible" /> to false.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return this._isSelected;
            }

            set
            {
                this._isSelected = value;

                /*
				if ( this is BarItem )
				{
					( (BarItem)this ).Bar.Fill.UseInactiveColor = !value;
				}
				else if ( this is LineItem )
				{
					( (LineItem)this ).Line.Fill.UseInactiveColor = !value;
					( (LineItem)this ).Symbol.Fill.UseInactiveColor = !value;
					( (LineItem)this ).Symbol.Fill.UseInactiveColor = !value;
				}
				else if ( this is HiLowBarItem )
					( (HiLowBarItem)this ).Bar.Fill.UseInactiveColor = !value;
				else if ( this is PieItem )
					( (PieItem)this ).Fill.UseInactiveColor = !value;
				*/
            }
        }

        // Revision: JCarpenter 10/06
        /// <summary>Determines whether this <see cref="CurveItem" /> can be selected in the graph.</summary>
        public bool IsSelectable
        {
            get
            {
                return this._isSelectable;
            }

            set
            {
                this._isSelectable = value;
            }
        }

        /// <summary>Gets or sets a value which allows you to override the normal ordinal axis behavior.</summary>
        /// <remarks>
        /// Normally for an ordinal axis type, the actual data values corresponding to the ordinal axis will be ignored (essentially they are replaced by ordinal
        /// values, e.g., 1, 2, 3, etc). If IsOverrideOrdinal is true, then the user data values will be used (even if they don't make sense).  Fractional values
        /// are allowed, such that a value of 1.5 is between the first and second ordinal position, etc.
        /// </remarks>
        /// <seealso cref="AxisType.Ordinal" />
        /// <seealso cref="AxisType.Text" />
        public bool IsOverrideOrdinal
        {
            get
            {
                return this._isOverrideOrdinal;
            }

            set
            {
                this._isOverrideOrdinal = value;
            }
        }

        /// <summary>Gets or sets a value that determines which X axis this <see cref="CurveItem" />
        /// is assigned to.</summary>
        /// <remarks>
        /// The
        /// <see cref="XAxis" /> is on the bottom side of the graph and the
        /// <see cref="X2Axis" /> is on the top side.  Assignment to an axis determines the scale that is used to draw the curve on the graph.
        /// </remarks>
        /// <value>true to assign the curve to the <see cref="X2Axis" />, false to assign the curve to the <see cref="XAxis" /></value>
        public bool IsX2Axis
        {
            get
            {
                return this._isX2Axis;
            }

            set
            {
                this._isX2Axis = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that determines which Y axis this <see cref="CurveItem" />
        /// is assigned to.
        /// </summary>
        /// <remarks>
        /// The
        /// <see cref="YAxis" /> is on the left side of the graph and the
        /// <see cref="Y2Axis" /> is on the right side.  Assignment to an axis determines the scale that is used to draw the curve on the graph.  Note that this
        /// value is used in combination with the <see cref="YAxisIndex" /> to determine which of the Y Axes (if there are multiples) this curve belongs to.
        /// </remarks>
        /// <value>true to assign the curve to the <see cref="Y2Axis" />, false to assign the curve to the <see cref="YAxis" /></value>
        public bool IsY2Axis
        {
            get
            {
                return this._isY2Axis;
            }

            set
            {
                this._isY2Axis = value;
            }
        }

        /// <summary>Gets or sets the index number of the Y Axis to which this
        /// <see cref="CurveItem" /> belongs.</summary>
        /// <remarks>
        /// This value is essentially an index number into the <see cref="GraphPane.YAxisList" />
        /// or <see cref="GraphPane.Y2AxisList" />, depending on the setting of
        /// <see cref="IsY2Axis" />.
        /// </remarks>
        public int YAxisIndex
        {
            get
            {
                return this._yAxisIndex;
            }

            set
            {
                this._yAxisIndex = value;
            }
        }

        /// <summary>Determines whether this <see cref="CurveItem" />
        /// is a <see cref="BarItem" />.</summary>
        /// <value>true for a bar chart, or false for a line or pie graph</value>
        public bool IsBar
        {
            get
            {
                return this is BarItem || this is HiLowBarItem || this is ErrorBarItem;
            }
        }

        /// <summary>Determines whether this <see cref="CurveItem" />
        /// is a <see cref="PieItem" />.</summary>
        /// <value>true for a pie chart, or false for a line or bar graph</value>
        public bool IsPie
        {
            get
            {
                return this is PieItem;
            }
        }

        /// <summary>Determines whether this <see cref="CurveItem" />
        /// is a <see cref="LineItem" />.</summary>
        /// <value>true for a line chart, or false for a bar type</value>
        public bool IsLine
        {
            get
            {
                return this is LineItem;
            }
        }

        /// <summary>
        /// Gets a flag indicating if the Z data range should be included in the axis scaling calculations.
        /// </summary>
        /// <param name="pane">
        /// The parent <see cref="GraphPane"/> of this <see cref="CurveItem"/>.
        /// </param>
        /// <value>
        /// true if the Z data are included, false otherwise
        /// </value>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal abstract bool IsZIncluded(GraphPane pane);

        /// <summary>
        /// Gets a flag indicating if the X axis is the independent axis for this <see cref="CurveItem"/>
        /// </summary>
        /// <param name="pane">
        /// The parent <see cref="GraphPane"/> of this <see cref="CurveItem"/>.
        /// </param>
        /// <value>
        /// true if the X axis is independent, false otherwise
        /// </value>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal abstract bool IsXIndependent(GraphPane pane);

        /// <summary>
        /// Readonly property that gives the number of points that define this
        /// <see cref="CurveItem" /> object, which is the number of points in the
        /// <see cref="Points" /> data collection.
        /// </summary>
        public int NPts
        {
            get
            {
                if (this._points == null)
                {
                    return 0;
                }

                return this._points.Count;
            }
        }

        /// <summary>The <see cref="IPointList" /> of X,Y point sets that represent this
        /// <see cref="CurveItem" />.</summary>
        public IPointList Points
        {
            get
            {
                return this._points;
            }

            set
            {
                this._points = value;
            }
        }

        /// <summary>
        /// An accessor for the <see cref="PointPair"/> datum for this <see cref="CurveItem"/>. Index is the ordinal reference (zero based) of the
        /// point.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="PointPair"/>.
        /// </returns>
        public PointPair this[int index]
        {
            get
            {
                if (this._points == null)
                {
                    return new PointPair(PointPairBase.Missing, PointPairBase.Missing);
                }

                return this._points[index];
            }
        }

        /// <summary>Gets or sets the hyperlink information for this <see cref="CurveItem" />.</summary>
        // /// <seealso cref="ZedGraph.Web.IsImageMap" />
        public Link Link
        {
            get
            {
                return this._link;
            }

            set
            {
                this._link = value;
            }
        }

        #endregion

        #region Rendering Methods

        /// <summary>
        /// Do all rendering associated with this <see cref="CurveItem"/> to the specified
        /// <see cref="Graphics"/> device.  This method is normally only called by the Draw method of the parent <see cref="CurveList"/>
        /// collection object.
        /// </summary>
        /// <param name="g">
        /// A graphic device object to be drawn into.  This is normally e.Graphics from the PaintEventArgs argument to the Paint() method.
        /// </param>
        /// <param name="pane">
        /// A reference to the <see cref="GraphPane"/> object that is the parent or owner of this object.
        /// </param>
        /// <param name="pos">
        /// The ordinal position of the current <see cref="Bar"/>
        /// curve.
        /// </param>
        /// <param name="scaleFactor">
        /// The scaling factor to be used for rendering objects.  This is calculated and passed down by the parent <see cref="GraphPane"/> object using the
        /// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust font sizes, etc. according to the actual size of the graph.
        /// </param>
        public abstract void Draw(Graphics g, GraphPane pane, int pos, float scaleFactor);

        /// <summary>
        /// Draw a legend key entry for this <see cref="CurveItem"/> at the specified location. This abstract base method passes through to
        /// <see cref="BarItem.DrawLegendKey"/> or
        /// <see cref="LineItem.DrawLegendKey"/> to do the rendering.
        /// </summary>
        /// <param name="g">
        /// A graphic device object to be drawn into.  This is normally e.Graphics from the PaintEventArgs argument to the Paint() method.
        /// </param>
        /// <param name="pane">
        /// A reference to the <see cref="GraphPane"/> object that is the parent or owner of this object.
        /// </param>
        /// <param name="rect">
        /// The <see cref="RectangleF"/> struct that specifies the location for the legend key
        /// </param>
        /// <param name="scaleFactor">
        /// The scaling factor to be used for rendering objects.  This is calculated and passed down by the parent <see cref="GraphPane"/> object using the
        /// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust font sizes, etc. according to the actual size of the graph.
        /// </param>
        public abstract void DrawLegendKey(Graphics g, GraphPane pane, RectangleF rect, float scaleFactor);

        #endregion

        #region Utility Methods

        /// <summary>
        /// Add a single x,y coordinate point to the end of the points collection for this curve.
        /// </summary>
        /// <param name="x">
        /// The X coordinate value
        /// </param>
        /// <param name="y">
        /// The Y coordinate value
        /// </param>
        public void AddPoint(double x, double y)
        {
            this.AddPoint(new PointPair(x, y));
        }

        /// <summary>
        /// Add a <see cref="PointPair"/> object to the end of the points collection for this curve.
        /// </summary>
        /// <remarks>
        /// This method will only work if the <see cref="IPointList"/> instance reference at <see cref="Points"/> supports the <see cref="IPointListEdit"/>
        /// interface. Otherwise, it does nothing.
        /// </remarks>
        /// <param name="point">
        /// A reference to the <see cref="PointPair"/> object to be added
        /// </param>
        public void AddPoint(PointPair point)
        {
            if (this._points == null)
            {
                this.Points = new PointPairList();
            }

            if (this._points is IPointListEdit)
            {
                (this._points as IPointListEdit).Add(point);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>Clears the points from this <see cref="CurveItem" />.  This is the same as <c>CurveItem.Points.Clear()</c>.</summary>
        /// <remarks>
        /// This method will only work if the <see cref="IPointList" /> instance reference at <see cref="Points" /> supports the <see cref="IPointListEdit" />
        /// interface. Otherwise, it does nothing.
        /// </remarks>
        public void Clear()
        {
            if (this._points is IPointListEdit)
            {
                (this._points as IPointListEdit).Clear();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Removes a single point from this <see cref="CurveItem"/>.
        /// </summary>
        /// <remarks>
        /// This method will only work if the <see cref="IPointList"/> instance reference at <see cref="Points"/> supports the <see cref="IPointListEdit"/>
        /// interface. Otherwise, it does nothing.
        /// </remarks>
        /// <param name="index">
        /// The ordinal position of the point to be removed.
        /// </param>
        public void RemovePoint(int index)
        {
            if (this._points is IPointListEdit)
            {
                (this._points as IPointListEdit).RemoveAt(index);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Get the X Axis instance (either <see cref="XAxis"/> or <see cref="X2Axis"/>) to which this <see cref="CurveItem"/> belongs.
        /// </summary>
        /// <param name="pane">
        /// The <see cref="GraphPane"/> object to which this curve belongs.
        /// </param>
        /// <returns>
        /// Either a <see cref="XAxis"/> or <see cref="X2Axis"/> to which this
        /// <see cref="CurveItem"/> belongs.
        /// </returns>
        public Axis GetXAxis(GraphPane pane)
        {
            if (this._isX2Axis)
            {
                return pane.X2Axis;
            }

            return pane.XAxis;
        }

        /// <summary>
        /// Get the Y Axis instance (either <see cref="YAxis"/> or <see cref="Y2Axis"/>) to which this <see cref="CurveItem"/> belongs.
        /// </summary>
        /// <remarks>
        /// This method safely retrieves a Y Axis instance from either the <see cref="GraphPane.YAxisList"/>
        /// or the <see cref="GraphPane.Y2AxisList"/> using the values of <see cref="YAxisIndex"/> and
        /// <see cref="IsY2Axis"/>.  If the value of <see cref="YAxisIndex"/> is out of bounds, the default <see cref="YAxis"/> or <see cref="Y2Axis"/> is
        /// used.
        /// </remarks>
        /// <param name="pane">
        /// The <see cref="GraphPane"/> object to which this curve belongs.
        /// </param>
        /// <returns>
        /// Either a <see cref="YAxis"/> or <see cref="Y2Axis"/> to which this
        /// <see cref="CurveItem"/> belongs.
        /// </returns>
        public Axis GetYAxis(GraphPane pane)
        {
            if (this._isY2Axis)
            {
                if (this._yAxisIndex < pane.Y2AxisList.Count)
                {
                    return pane.Y2AxisList[this._yAxisIndex];
                }

                return pane.Y2AxisList[0];
            }

            if (this._yAxisIndex < pane.YAxisList.Count)
            {
                return pane.YAxisList[this._yAxisIndex];
            }

            return pane.YAxisList[0];
        }

        /// <summary>
        /// Get the index of the Y Axis in the <see cref="YAxis"/> or <see cref="Y2Axis"/> list to which this <see cref="CurveItem"/> belongs.
        /// </summary>
        /// <remarks>
        /// This method safely retrieves a Y Axis index into either the <see cref="GraphPane.YAxisList"/>
        /// or the <see cref="GraphPane.Y2AxisList"/> using the values of <see cref="YAxisIndex"/> and
        /// <see cref="IsY2Axis"/>.  If the value of <see cref="YAxisIndex"/> is out of bounds, the default <see cref="YAxis"/> or <see cref="Y2Axis"/> is
        /// used, which is index zero.
        /// </remarks>
        /// <param name="pane">
        /// The <see cref="GraphPane"/> object to which this curve belongs.
        /// </param>
        /// <returns>
        /// An integer value indicating which index position in the list applies to this
        /// <see cref="CurveItem"/>
        /// </returns>
        public int GetYAxisIndex(GraphPane pane)
        {
            if (this._yAxisIndex >= 0 && this._yAxisIndex < (this._isY2Axis ? pane.Y2AxisList.Count : pane.YAxisList.Count))
            {
                return this._yAxisIndex;
            }

            return 0;
        }

        /// <summary>
        /// Loads some pseudo unique colors/symbols into this CurveItem.  This is the same as <c>MakeUnique(ColorSymbolRotator.StaticInstance)</c>.
        /// <seealso cref="ColorSymbolRotator.StaticInstance" />
        /// <seealso cref="ColorSymbolRotator" />
        /// <seealso cref="MakeUnique(ColorSymbolRotator)" />
        /// </summary>
        public void MakeUnique()
        {
            this.MakeUnique(ColorSymbolRotator.StaticInstance);
        }

        /// <summary>
        /// Loads some pseudo unique colors/symbols into this CurveItem.  This is mainly useful for differentiating a set of new CurveItems without having to
        /// pick your own colors/symbols.
        /// <seealso cref="MakeUnique(ColorSymbolRotator)"/>
        /// </summary>
        /// <param name="rotator">
        /// The <see cref="ColorSymbolRotator"/> that is used to pick the color and symbol for this method call.
        /// </param>
        public virtual void MakeUnique(ColorSymbolRotator rotator)
        {
            this.Color = rotator.NextColor;
        }

        /// <summary>
        /// Go through the list of <see cref="PointPair"/> data values for this <see cref="CurveItem"/>
        /// and determine the minimum and maximum values in the data.
        /// </summary>
        /// <param name="xMin">
        /// The minimum X value in the range of data
        /// </param>
        /// <param name="xMax">
        /// The maximum X value in the range of data
        /// </param>
        /// <param name="yMin">
        /// The minimum Y value in the range of data
        /// </param>
        /// <param name="yMax">
        /// The maximum Y value in the range of data
        /// </param>
        /// <param name="ignoreInitial">
        /// ignoreInitial is a boolean value that affects the data range that is considered for the automatic scale ranging (see
        /// <see cref="GraphPane.IsIgnoreInitial"/>).  If true, then initial data points where the Y value is zero are not included when automatically
        /// determining the scale <see cref="Scale.Min"/>,
        /// <see cref="Scale.Max"/>, and <see cref="Scale.MajorStep"/> size.  All data after the first non-zero Y value are included.
        /// </param>
        /// <param name="isBoundedRanges">
        /// Determines if the auto-scaled axis ranges will subset the data points based on any manually set scale range values.
        /// </param>
        /// <param name="pane">
        /// A reference to the <see cref="GraphPane"/> object that is the parent or owner of this object.
        /// </param>
        /// <seealso cref="GraphPane.IsBoundedRanges"/>
        public virtual void GetRange(
            out double xMin, 
            out double xMax, 
            out double yMin, 
            out double yMax, 
            bool ignoreInitial, 
            bool isBoundedRanges, 
            GraphPane pane)
        {
            // The lower and upper bounds of allowable data for the X values.  These
            // values allow you to subset the data values.  If the X range is bounded, then
            // the resulting range for Y will reflect the Y values for the points within the X
            // bounds.
            double xLBound = double.MinValue;
            double xUBound = double.MaxValue;
            double yLBound = double.MinValue;
            double yUBound = double.MaxValue;

            // initialize the values to outrageous ones to start
            xMin = yMin = double.MaxValue;
            xMax = yMax = double.MinValue;

            Axis yAxis = this.GetYAxis(pane);
            Axis xAxis = this.GetXAxis(pane);
            if (yAxis == null || xAxis == null)
            {
                return;
            }

            if (isBoundedRanges)
            {
                xLBound = xAxis._scale._lBound;
                xUBound = xAxis._scale._uBound;
                yLBound = yAxis._scale._lBound;
                yUBound = yAxis._scale._uBound;
            }

            bool isZIncluded = this.IsZIncluded(pane);
            bool isXIndependent = this.IsXIndependent(pane);
            bool isXLog = xAxis.Scale.IsLog;
            bool isYLog = yAxis.Scale.IsLog;
            bool isXOrdinal = xAxis.Scale.IsAnyOrdinal;
            bool isYOrdinal = yAxis.Scale.IsAnyOrdinal;
            bool isZOrdinal = (isXIndependent ? yAxis : xAxis).Scale.IsAnyOrdinal;

            // Loop over each point in the arrays
            // foreach ( PointPair point in this.Points )
            for (int i = 0; i < this.Points.Count; i++)
            {
                PointPair point = this.Points[i];

                double curX = isXOrdinal ? i + 1 : point.X;
                double curY = isYOrdinal ? i + 1 : point.Y;
                double curZ = isZOrdinal ? i + 1 : point.Z;

                bool outOfBounds = curX < xLBound || curX > xUBound || curY < yLBound || curY > yUBound
                                   || (isZIncluded && isXIndependent && (curZ < yLBound || curZ > yUBound))
                                   || (isZIncluded && !isXIndependent && (curZ < xLBound || curZ > xUBound)) || (curX <= 0 && isXLog)
                                   || (curY <= 0 && isYLog);

                // ignoreInitial becomes false at the first non-zero
                // Y value
                if (ignoreInitial && curY != 0 && curY != PointPairBase.Missing)
                {
                    ignoreInitial = false;
                }

                if (!ignoreInitial && !outOfBounds && curX != PointPairBase.Missing && curY != PointPairBase.Missing)
                {
                    if (curX < xMin)
                    {
                        xMin = curX;
                    }

                    if (curX > xMax)
                    {
                        xMax = curX;
                    }

                    if (curY < yMin)
                    {
                        yMin = curY;
                    }

                    if (curY > yMax)
                    {
                        yMax = curY;
                    }

                    if (isZIncluded && isXIndependent && curZ != PointPairBase.Missing)
                    {
                        if (curZ < yMin)
                        {
                            yMin = curZ;
                        }

                        if (curZ > yMax)
                        {
                            yMax = curZ;
                        }
                    }
                    else if (isZIncluded && curZ != PointPairBase.Missing)
                    {
                        if (curZ < xMin)
                        {
                            xMin = curZ;
                        }

                        if (curZ > xMax)
                        {
                            xMax = curZ;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns a reference to the <see cref="Axis"/> object that is the "base" (independent axis) from which the values are drawn. 
        /// </summary>
        /// <param name="pane">
        /// The pane.
        /// </param>
        /// <remarks>
        /// This property is determined by the value of <see cref="BarSettings.Base"/> for
        /// <see cref="BarItem"/>, <see cref="ErrorBarItem"/>, and <see cref="HiLowBarItem"/>
        /// types.  It is always the X axis for regular <see cref="LineItem"/> types. Note that the <see cref="BarSettings.Base"/> setting can override the
        /// <see cref="IsY2Axis"/> and <see cref="YAxisIndex"/> settings for bar types (this is because all the bars that are clustered together must share the
        /// same base axis).
        /// </remarks>
        /// <seealso cref="BarBase"/>
        /// <seealso cref="ValueAxis"/>
        /// <returns>
        /// The <see cref="Axis"/>.
        /// </returns>
        public virtual Axis BaseAxis(GraphPane pane)
        {
            BarBase barBase;

            if (this is BarItem || this is ErrorBarItem || this is HiLowBarItem)
            {
                barBase = pane._barSettings.Base;
            }
            else
            {
                barBase = this._isX2Axis ? BarBase.X2 : BarBase.X;
            }

            if (barBase == BarBase.X)
            {
                return pane.XAxis;
            }

            if (barBase == BarBase.X2)
            {
                return pane.X2Axis;
            }

            if (barBase == BarBase.Y)
            {
                return pane.YAxis;
            }

            return pane.Y2Axis;
        }

        /// <summary>
        /// Returns a reference to the <see cref="Axis"/> object that is the "value" (dependent axis) from which the points are drawn. 
        /// </summary>
        /// <param name="pane">
        /// The pane.
        /// </param>
        /// <remarks>
        /// This property is determined by the value of <see cref="BarSettings.Base"/> for
        /// <see cref="BarItem"/>, <see cref="ErrorBarItem"/>, and <see cref="HiLowBarItem"/>
        /// types.  It is always the Y axis for regular <see cref="LineItem"/> types.
        /// </remarks>
        /// <seealso cref="BarBase"/>
        /// <seealso cref="BaseAxis"/>
        /// <returns>
        /// The <see cref="Axis"/>.
        /// </returns>
        public virtual Axis ValueAxis(GraphPane pane)
        {
            BarBase barBase;

            if (this is BarItem || this is ErrorBarItem || this is HiLowBarItem)
            {
                barBase = pane._barSettings.Base;
            }
            else
            {
                barBase = BarBase.X;
            }

            if (barBase == BarBase.X || barBase == BarBase.X2)
            {
                return this.GetYAxis(pane);
            }

            return this.GetXAxis(pane);
        }

        /// <summary>
        /// Calculate the width of each bar, depending on the actual bar type
        /// </summary>
        /// <param name="pane">
        /// The pane.
        /// </param>
        /// <returns>
        /// The width for an individual bar, in pixel units
        /// </returns>
        public float GetBarWidth(GraphPane pane)
        {
            // Total axis width = 
            // npts * ( nbars * ( bar + bargap ) - bargap + clustgap )
            // cg * bar = cluster gap
            // npts = max number of points in any curve
            // nbars = total number of curves that are of type IsBar
            // bar = bar width
            // bg * bar = bar gap
            // therefore:
            // totwidth = npts * ( nbars * (bar + bg*bar) - bg*bar + cg*bar )
            // totwidth = bar * ( npts * ( nbars * ( 1 + bg ) - bg + cg ) )
            // solve for bar
            float barWidth;

            if (this is ErrorBarItem)
            {
                barWidth = ((ErrorBarItem)this).Bar.Symbol.Size * pane.CalcScaleFactor();
            }

            // 			else if ( this is HiLowBarItem && pane._barSettings.Type != BarType.ClusterHiLow )
            // 				barWidth = (float) ( ((HiLowBarItem)this).Bar.GetBarWidth( pane,
            // 						((HiLowBarItem)this).BaseAxis(pane), pane.CalcScaleFactor() ) );
            // 				barWidth = (float) ( ((HiLowBarItem)this).Bar.Size *
            // 						pane.CalcScaleFactor() );
            else
            {
                // BarItem or LineItem
                // For stacked bar types, the bar width will be based on a single bar
                float numBars = 1.0F;
                if (pane._barSettings.Type == BarType.Cluster)
                {
                    numBars = pane.CurveList.NumClusterableBars;
                }

                float denom = numBars * (1.0F + pane._barSettings.MinBarGap) - pane._barSettings.MinBarGap + pane._barSettings.MinClusterGap;
                if (denom <= 0)
                {
                    denom = 1;
                }

                barWidth = pane.BarSettings.GetClusterWidth() / denom;
            }

            if (barWidth <= 0)
            {
                return 1;
            }

            return barWidth;
        }

        /// <summary>
        /// Determine the coords for the rectangle associated with a specified point for this <see cref="CurveItem"/>
        /// </summary>
        /// <param name="pane">
        /// The <see cref="GraphPane"/> to which this curve belongs
        /// </param>
        /// <param name="i">
        /// The index of the point of interest
        /// </param>
        /// <param name="coords">
        /// A list of coordinates that represents the "rect" for this point (used in an html AREA tag)
        /// </param>
        /// <returns>
        /// true if it's a valid point, false otherwise
        /// </returns>
        public abstract bool GetCoords(GraphPane pane, int i, out string coords);

        #endregion

        #region Inner classes

#if ( DOTNET1 ) // Is this a .Net 1.1 compilation?
	
    // <summary>
    /// Compares <see cref="CurveItem"/>'s based on the point value at the specified
    /// index and for the specified axis.
    /// <seealso cref="System.Collections.ArrayList.Sort()"/>
    /// </summary>
		public class Comparer : IComparer
		{
			private int index;
			private SortType sortType;
			

			/// <summary>
			/// Constructor for Comparer.
			/// </summary>
			/// <param name="type">The axis type on which to sort.</param>
			/// <param name="index">The index number of the point on which to sort</param>
			public Comparer( SortType type, int index )
			{
				this.sortType = type;
				this.index = index;
			}
			

			/// <summary>
			/// Compares two <see cref="CurveItem"/>s using the previously specified index value
			/// and axis.  Sorts in descending order.
			/// </summary>
			/// <param name="l">Curve to the left.</param>
			/// <param name="r">Curve to the right.</param>
			/// <returns>-1, 0, or 1 depending on l.X's relation to r.X</returns>
			public int Compare( object l, object r ) 
			{
				CurveItem cl = (CurveItem) l;
				CurveItem cr = (CurveItem) r;

				if (cl == null && cr == null )
					return 0;
				else if (cl == null && cr != null ) 
					return -1;
				else if (cl != null && cr == null) 
					return 1;

				if ( cr != null && cr.NPts <= index )
					cr = null;
				if ( cl != null && cl.NPts <= index )
					cl = null;
						
				double lVal, rVal;

				if ( sortType == SortType.XValues )
				{
					lVal = ( l != null ) ? System.Math.Abs( cl[index].X ) : PointPair.Missing;
					rVal = ( r != null ) ? System.Math.Abs( cr[index].X ) : PointPair.Missing;
				}
				else
				{
					lVal = ( l != null ) ? System.Math.Abs( cl[index].Y ) : PointPair.Missing;
					rVal = ( r != null ) ? System.Math.Abs( cr[index].Y ) : PointPair.Missing;
				}
				
				if ( lVal == PointPair.Missing || Double.IsInfinity( lVal ) || Double.IsNaN( lVal ) )
					cl = null;
				if ( rVal == PointPair.Missing || Double.IsInfinity( rVal ) || Double.IsNaN( rVal ) )
					cr = null;
					
				if ( ( cl == null && cr == null) || ( System.Math.Abs( lVal - rVal ) < 1e-10 ) )
					return 0;
				else if ( cl == null && cr != null ) 
					return -1;
				else if ( cl != null && r == null ) 
					return 1;
				else
					return rVal < lVal ? -1 : 1;
			}
		}
	
#else // Otherwise, it's .Net 2.0 so use generics

        /// <summary>
        /// Compares <see cref="CurveItem" />'s based on the point value at the specified index and for the specified axis.
        /// <seealso cref="System.Collections.ArrayList.Sort()" />
        /// </summary>
        public class Comparer : IComparer<CurveItem>
        {
            /// <summary>
            /// The index.
            /// </summary>
            private int index;

            /// <summary>
            /// The sort type.
            /// </summary>
            private SortType sortType;

            /// <summary>
            /// Initializes a new instance of the <see cref="Comparer"/> class. 
            /// Constructor for Comparer.
            /// </summary>
            /// <param name="type">
            /// The axis type on which to sort.
            /// </param>
            /// <param name="index">
            /// The index number of the point on which to sort
            /// </param>
            public Comparer(SortType type, int index)
            {
                this.sortType = type;
                this.index = index;
            }

            /// <summary>
            /// Compares two <see cref="CurveItem"/>s using the previously specified index value and axis.  Sorts in descending order.
            /// </summary>
            /// <param name="l">
            /// Curve to the left.
            /// </param>
            /// <param name="r">
            /// Curve to the right.
            /// </param>
            /// <returns>
            /// -1, 0, or 1 depending on l.X's relation to r.X
            /// </returns>
            public int Compare(CurveItem l, CurveItem r)
            {
                if (l == null && r == null)
                {
                    return 0;
                }

                if (l == null && r != null)
                {
                    return -1;
                }

                if (l != null && r == null)
                {
                    return 1;
                }

                if (r != null && r.NPts <= this.index)
                {
                    r = null;
                }

                if (l != null && l.NPts <= this.index)
                {
                    l = null;
                }

                double lVal, rVal;

                if (this.sortType == SortType.XValues)
                {
                    lVal = (l != null) ? Math.Abs(l[this.index].X) : PointPairBase.Missing;
                    rVal = (r != null) ? Math.Abs(r[this.index].X) : PointPairBase.Missing;
                }
                else
                {
                    lVal = (l != null) ? Math.Abs(l[this.index].Y) : PointPairBase.Missing;
                    rVal = (r != null) ? Math.Abs(r[this.index].Y) : PointPairBase.Missing;
                }

                if (lVal == PointPairBase.Missing || double.IsInfinity(lVal) || double.IsNaN(lVal))
                {
                    l = null;
                }

                if (rVal == PointPairBase.Missing || double.IsInfinity(rVal) || double.IsNaN(rVal))
                {
                    r = null;
                }

                if ((l == null && r == null) || (Math.Abs(lVal - rVal) < 1e-10))
                {
                    return 0;
                }

                if (l == null && r != null)
                {
                    return -1;
                }

                if (l != null && r == null)
                {
                    return 1;
                }

                return rVal < lVal ? -1 : 1;
            }
        }

#endif

        #endregion
    }
}