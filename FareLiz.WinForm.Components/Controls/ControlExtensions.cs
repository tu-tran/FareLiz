namespace SkyDean.FareLiz.WinForm.Components.Controls
{
    #region

    using System;
    using System.Drawing;
    using System.Windows.Forms;

    #endregion

    /// <summary>The control extensions.</summary>
    public static class ControlExtensions
    {
        #region Public Methods and Operators

        /// <summary>Asynchronously invoke with validation check</summary>
        /// <returns>The <see cref="IAsyncResult"/>.</returns>
        public static IAsyncResult BeginSafeInvoke(this Control target, Delegate method, params object[] args)
        {
            if (CanInvoke(target))
            {
                return target.BeginInvoke(method, args);
            }

            return null;
        }

        /// <summary>Asynchnorously invoke with validation check</summary>
        /// <returns>The <see cref="IAsyncResult"/>.</returns>
        public static IAsyncResult BeginSafeInvoke(this Control target, Delegate method)
        {
            return BeginSafeInvoke(target, method, null);
        }

        /// <summary>Check if the control can be invoked (check if the control handled is created and not yet disposed)</summary>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool CanInvoke(this Control target)
        {
            return !target.IsUnusable();
        }

        /// <summary>Measure the size of the text assosiated with the control</summary>
        /// <returns>The <see cref="Size"/>.</returns>
        public static Size GetTextSize(this Control target)
        {
            return string.IsNullOrEmpty(target.Text) ? Size.Empty : TextRenderer.MeasureText(target.Text, target.Font);
        }

        /// <summary>Check if the control is disposed or being disposed</summary>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool IsDestructed(this Control target)
        {
            return target.IsDisposed || target.Disposing;
        }

        /// <summary>Check if the control is no longer usable (check if the control handled is created and not yet disposed)</summary>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool IsUnusable(this Control target)
        {
            return target.IsDestructed() || !target.IsHandleCreated;
        }

        /// <summary>Invoke with disposed check</summary>
        /// <returns>The <see cref="object"/>.</returns>
        public static object SafeInvoke(this Control target, Delegate method, params object[] args)
        {
            if (target.CanInvoke())
            {
                return target.Invoke(method, args);
            }

            return null;
        }

        /// <summary>Invoke with null and disposed check</summary>
        /// <returns>The <see cref="object"/>.</returns>
        public static object SafeInvoke(this Control target, Delegate method)
        {
            return target.SafeInvoke(method, null);
        }

        /// <summary>Invoke with null and disposed check if needed</summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="evaluation">The evaluation method.</param>
        /// <param name="args">The evaluation arguments.</param>
        /// <returns>The return value of the <paramref name="evaluation"/>.</returns>
        public static T InvokeIfNeeded<T>(this Control target, Func<T> evaluation, params object[] args)
        {
            if (target.InvokeRequired)
                return (T)target.SafeInvoke(evaluation);
            return (T)evaluation.DynamicInvoke(args);
        }

        /// <summary>Invoke with null and disposed check if needed</summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="evaluation">The evaluation method.</param>
        /// <param name="args">The evaluation arguments.</param>
        /// <returns>The return value of the <paramref name="evaluation"/>.</returns>
        public static void InvokeActionIfNeeded(this Control target, Delegate evaluation, params object[] args)
        {
            if (target.InvokeRequired)
                target.SafeInvoke(evaluation);
            else
                evaluation.DynamicInvoke(args);
        }

        #endregion
    }
}