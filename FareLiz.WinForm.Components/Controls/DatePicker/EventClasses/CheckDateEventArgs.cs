﻿namespace SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.EventClasses
{
    using System;

    using SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.DatePicker;

    /// <summary>
   /// Class that holds information for the <see cref="EnhancedDatePicker.ValueChanged"/> event.
   /// </summary>
   public class CheckDateEventArgs : EventArgs
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="CheckDateEventArgs"/> class.
      /// </summary>
      /// <param name="date">The date value.</param>
      /// <param name="valid">true if it is a valid date; false otherwise.</param>
      public CheckDateEventArgs(DateTime date, bool valid)
      {
         this.Date = date;
         this.IsValid = valid;
      }

      /// <summary>
      /// Gets the date.
      /// </summary>
      public DateTime Date { get; private set; }

      /// <summary>
      /// Gets or sets a value indicating whether the <see cref="Date"/> is valid.
      /// </summary>
      public bool IsValid { get; set; }
   }
}