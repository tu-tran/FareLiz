namespace SkyDean.FareLiz.Data.Csv
{
    using System;
    using System.ComponentModel;

    /// <summary>The cached csv reader.</summary>
    public partial class CachedCsvReader : CsvReader
    {
        /// <summary>Represents a CSV field property descriptor.</summary>
        private class CsvPropertyDescriptor : PropertyDescriptor
        {
            #region Fields

            /// <summary>Contains the field index.</summary>
            private readonly int _index;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the CsvPropertyDescriptor class.
            /// </summary>
            /// <param name="fieldName">
            /// The field name.
            /// </param>
            /// <param name="index">
            /// The field index.
            /// </param>
            public CsvPropertyDescriptor(string fieldName, int index)
                : base(fieldName, null)
            {
                this._index = index;
            }

            #endregion

            #region Properties

            /// <summary>Gets the field index.</summary>
            /// <value>The field index.</value>
            public int Index
            {
                get
                {
                    return this._index;
                }
            }

            #endregion

            #region Overrides

            /// <summary>
            /// The can reset value.
            /// </summary>
            /// <param name="component">
            /// The component.
            /// </param>
            /// <returns>
            /// The <see cref="bool"/>.
            /// </returns>
            public override bool CanResetValue(object component)
            {
                return false;
            }

            /// <summary>
            /// The get value.
            /// </summary>
            /// <param name="component">
            /// The component.
            /// </param>
            /// <returns>
            /// The <see cref="object"/>.
            /// </returns>
            public override object GetValue(object component)
            {
                return ((string[])component)[this._index];
            }

            /// <summary>
            /// The reset value.
            /// </summary>
            /// <param name="component">
            /// The component.
            /// </param>
            public override void ResetValue(object component)
            {
            }

            /// <summary>
            /// The set value.
            /// </summary>
            /// <param name="component">
            /// The component.
            /// </param>
            /// <param name="value">
            /// The value.
            /// </param>
            public override void SetValue(object component, object value)
            {
            }

            /// <summary>
            /// The should serialize value.
            /// </summary>
            /// <param name="component">
            /// The component.
            /// </param>
            /// <returns>
            /// The <see cref="bool"/>.
            /// </returns>
            public override bool ShouldSerializeValue(object component)
            {
                return false;
            }

            /// <summary>Gets the component type.</summary>
            public override Type ComponentType
            {
                get
                {
                    return typeof(CachedCsvReader);
                }
            }

            /// <summary>Gets a value indicating whether is read only.</summary>
            public override bool IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            /// <summary>Gets the property type.</summary>
            public override Type PropertyType
            {
                get
                {
                    return typeof(string);
                }
            }

            #endregion
        }
    }
}