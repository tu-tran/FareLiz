﻿namespace SkyDean.FareLiz.WinForm.Components.Controls.DatePicker
{
    using System;

    /// <summary>
   /// Struct to store a bolded date.
   /// </summary>
   public struct BoldedDate
   {
      #region Properties

      /// <summary>
      /// Gets or sets the category of the bolded date.
      /// </summary>
      public BoldedDateCategory Category { get; set; }

      /// <summary>
      /// Gets or sets the <see cref="DateTime"/> value.
      /// </summary>
      public DateTime Value { get; set; }

      /// <summary>
      /// Gets a value indicating whether this instance is empty/invalid.
      /// </summary>
      public bool IsEmpty
      {
         get { return this.Category.IsEmpty || this.Value == DateTime.MinValue; }
      }

      #endregion
   }
}