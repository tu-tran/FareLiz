//============================================================================
//ZedGraph Class Library - A Flexible Line Graph/Bar Graph Library in C#
//Copyright � 2005  John Champion
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
namespace SkyDean.FareLiz.WinForm.Components.Controls.Graph
{
    using System;
    using System.Drawing;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    /// <summary>The LinearScale class inherits from the <see cref="Scale" /> class, and implements the features specific to <see cref="AxisType.Linear" />.</summary>
    /// <remarks>LinearScale is the normal, default cartesian axis.</remarks>
    /// <author> John Champion  </author>
    /// <version> $Revision: 1.10 $ $Date: 2007-04-16 00:03:02 $ </version>
    [Serializable]
    internal class LinearScale : Scale, ISerializable
    {
        // , ICloneable
        #region Properties

        /// <summary>Return the <see cref="AxisType" /> for this <see cref="Scale" />, which is
        /// <see cref="AxisType.Linear" />.</summary>
        public override AxisType Type
        {
            get
            {
                return AxisType.Linear;
            }
        }

        #endregion

        #region methods

        /// <summary>
        /// Select a reasonable linear axis scale given a range of data values.
        /// </summary>
        /// <remarks>
        /// This method only applies to <see cref="AxisType.Linear"/> type axes, and it is called by the general <see cref="Scale.PickScale"/> method.  The
        /// scale range is chosen based on increments of 1, 2, or 5 (because they are even divisors of 10).  This method honors the <see cref="Scale.MinAuto"/>,
        /// <see cref="Scale.MaxAuto"/>, and <see cref="Scale.MajorStepAuto"/> autorange settings. In the event that any of the autorange settings are false,
        /// the corresponding <see cref="Scale.Min"/>, <see cref="Scale.Max"/>, or <see cref="Scale.MajorStep"/>
        /// setting is explicitly honored, and the remaining autorange settings (if any) will be calculated to accomodate the non-autoranged values.  The basic
        /// defaults for scale selection are defined using <see cref="Scale.Default.ZeroLever"/>,
        /// <see cref="Scale.Default.TargetXSteps"/>, and <see cref="Scale.Default.TargetYSteps"/>
        /// from the <see cref="Scale.Default"/> default class.
        /// <para>
        /// On Exit:
        /// </para>
        /// <para>
        /// <see cref="Scale.Min"/> is set to scale minimum (if <see cref="Scale.MinAuto"/> = true)
        /// </para>
        /// <para>
        /// <see cref="Scale.Max"/> is set to scale maximum (if <see cref="Scale.MaxAuto"/> = true)
        /// </para>
        /// <para>
        /// <see cref="Scale.MajorStep"/> is set to scale step size (if <see cref="Scale.MajorStepAuto"/> = true)
        /// </para>
        /// <para>
        /// <see cref="Scale.MinorStep"/> is set to scale minor step size (if <see cref="Scale.MinorStepAuto"/> = true)
        /// </para>
        /// <para>
        /// <see cref="Scale.Mag"/> is set to a magnitude multiplier according to the data
        /// </para>
        /// <para>
        /// <see cref="Scale.Format"/> is set to the display format for the values (this controls the number of decimal places, whether there are thousands
        /// separators, currency types, etc.)
        /// </para>
        /// </remarks>
        /// <param name="pane">
        /// A reference to the <see cref="GraphPane"/> object associated with this <see cref="Axis"/>
        /// </param>
        /// <param name="g">
        /// A graphic device object to be drawn into.  This is normally e.Graphics from the PaintEventArgs argument to the Paint() method.
        /// </param>
        /// <param name="scaleFactor">
        /// The scaling factor to be used for rendering objects.  This is calculated and passed down by the parent <see cref="GraphPane"/> object using the
        /// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust font sizes, etc. according to the actual size of the graph.
        /// </param>
        /// <seealso cref="PickScale"/>
        /// <seealso cref="AxisType.Linear"/>
        public override void PickScale(GraphPane pane, Graphics g, float scaleFactor)
        {
            // call the base class first
            base.PickScale(pane, g, scaleFactor);

            // Test for trivial condition of range = 0 and pick a suitable default
            if (this._max - this._min < 1.0e-30)
            {
                if (this._maxAuto)
                {
                    this._max = this._max + 0.2 * (this._max == 0 ? 1.0 : Math.Abs(this._max));
                }

                if (this._minAuto)
                {
                    this._min = this._min - 0.2 * (this._min == 0 ? 1.0 : Math.Abs(this._min));
                }
            }

            // This is the zero-lever test.  If minVal is within the zero lever fraction
            // of the data range, then use zero.
            if (this._minAuto && this._min > 0 && this._min / (this._max - this._min) < Default.ZeroLever)
            {
                this._min = 0;
            }

            // Repeat the zero-lever test for cases where the maxVal is less than zero
            if (this._maxAuto && this._max < 0 && Math.Abs(this._max / (this._max - this._min)) < Default.ZeroLever)
            {
                this._max = 0;
            }

            // Calculate the new step size
            if (this._majorStepAuto)
            {
                double targetSteps = (this._ownerAxis is XAxis || this._ownerAxis is X2Axis) ? Default.TargetXSteps : Default.TargetYSteps;

                // Calculate the step size based on target steps
                this._majorStep = CalcStepSize(this._max - this._min, targetSteps);

                if (this._isPreventLabelOverlap)
                {
                    // Calculate the maximum number of labels
                    double maxLabels = this.CalcMaxLabels(g, pane, scaleFactor);

                    if (maxLabels < (this._max - this._min) / this._majorStep)
                    {
                        this._majorStep = this.CalcBoundedStepSize(this._max - this._min, maxLabels);
                    }
                }
            }

            // Calculate the new step size
            if (this._minorStepAuto)
            {
                this._minorStep = CalcStepSize(
                    this._majorStep, 
                    (this._ownerAxis is XAxis || this._ownerAxis is X2Axis) ? Default.TargetMinorXSteps : Default.TargetMinorYSteps);
            }

            // Calculate the scale minimum
            if (this._minAuto)
            {
                this._min = this._min - this.MyMod(this._min, this._majorStep);
            }

            // Calculate the scale maximum
            if (this._maxAuto)
            {
                this._max = this.MyMod(this._max, this._majorStep) == 0.0
                                ? this._max
                                : this._max + this._majorStep - this.MyMod(this._max, this._majorStep);
            }

            this.SetScaleMag(this._min, this._max, this._majorStep);
        }

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearScale"/> class. 
        /// Default constructor that defines the owner <see cref="Axis"/>
        /// (containing object) for this new object.
        /// </summary>
        /// <param name="owner">
        /// The owner, or containing object, of this instance
        /// </param>
        public LinearScale(Axis owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearScale"/> class. 
        /// The Copy Constructor
        /// </summary>
        /// <param name="rhs">
        /// The <see cref="LinearScale"/> object from which to copy
        /// </param>
        /// <param name="owner">
        /// The <see cref="Axis"/> object that will own the new instance of <see cref="LinearScale"/>
        /// </param>
        public LinearScale(Scale rhs, Axis owner)
            : base(rhs, owner)
        {
        }

        /// <summary>
        /// Create a new clone of the current item, with a new owner assignment
        /// </summary>
        /// <param name="owner">
        /// The new <see cref="Axis"/> instance that will be the owner of the new Scale
        /// </param>
        /// <returns>
        /// A new <see cref="Scale"/> clone.
        /// </returns>
        public override Scale Clone(Axis owner)
        {
            return new LinearScale(this, owner);
        }

        #endregion

        #region Serialization

        /// <summary>Current schema value that defines the version of the serialized file</summary>
        public const int schema2 = 10;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearScale"/> class. 
        /// Constructor for deserializing objects
        /// </summary>
        /// <param name="info">
        /// A <see cref="SerializationInfo"/> instance that defines the serialized data
        /// </param>
        /// <param name="context">
        /// A <see cref="StreamingContext"/> instance that contains the serialized data
        /// </param>
        protected LinearScale(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // The schema value is just a file version parameter.  You can use it to make future versions
            // backwards compatible as new member variables are added to classes
            int sch = info.GetInt32("schema2");
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
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("schema2", schema2);
        }

        #endregion
    }
}