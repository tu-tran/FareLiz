using System;

namespace SkyDean.FareLiz.Core.Presentation
{
    public enum ProgressStyle { Continuous, Marquee }    

    public delegate void CallbackDelegate(IProgressCallback callback);
    public delegate void CallbackExceptionDelegate(IProgressCallback callback, Exception data);

    public delegate void IncrementInvoker(int val);
    public delegate void RangeInvoker(int minimum, int maximum);
    public delegate void SetTextInvoker(String text);
    public delegate void StepToInvoker(int val);
    public delegate void StyleInvoker(ProgressStyle style);

    /// <summary>
    ///     This defines an interface which can be implemented by UI elements
    ///     which indicate the progress of a long operation.
    ///     (See ProgressDialog for a typical implementation)
    /// </summary>
    public interface IProgressCallback : INotification
    {
        /// <summary>
        ///     If this property is true, then you should abort work
        /// </summary>
        /// <remarks>You must have called one of the Begin() methods prior to this call.</remarks>
        bool IsAborting { get; }

        /// <summary>
        ///     Call this method from the worker thread to initialize
        ///     the progress callback.
        /// </summary>
        /// <param name="minimum">The minimum value in the progress range (e.g. 0)</param>
        /// <param name="maximum">The maximum value in the progress range (e.g. 100)</param>
        void Begin(int minimum, int maximum);

        /// <summary>
        ///     Call this method from the worker thread to initialize
        ///     the progress callback, without setting the range
        /// </summary>
        void Begin();

        /// <summary>
        ///     Call this method from the worker thread to reset the range in the progress callback
        /// </summary>
        /// <param name="minimum">The minimum value in the progress range (e.g. 0)</param>
        /// <param name="maximum">The maximum value in the progress range (e.g. 100)</param>
        /// <remarks>You must have called one of the Begin() methods prior to this call.</remarks>
        void SetRange(int minimum, int maximum);

        /// <summary>
        /// Get or set the progress text.
        /// </summary>
        /// <remarks>You must have called one of the Begin() methods prior to this call.</remarks>
        string Text { get; set; }

        /// <summary>
        /// Get or set the progress style
        /// </summary>
        ProgressStyle Style { get; set; }

        /// <summary>
        /// Get or set the title.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        ///     Call this method from the worker thread to append the progress text.
        /// </summary>
        /// <param name="text">The progress text to display</param>
        /// <remarks>You must have called one of the Begin() methods prior to this call.</remarks>
        void AppendText(String text);

        /// <summary>
        ///     Call this method from the worker thread to increase the progress counter by a specified value.
        /// </summary>
        /// <param name="val">The amount by which to increment the progress indicator</param>
        /// <remarks>You must have called one of the Begin() methods prior to this call.</remarks>
        void StepTo(int val);

        /// <summary>
        ///     Call this method from the worker thread to step the progress meter to a particular value.
        /// </summary>
        /// <param name="val">The value to which to step the meter</param>
        /// <remarks>You must have called one of the Begin() methods prior to this call.</remarks>
        void Increment(int val);

        /// <summary>
        ///     Call this method from the worker thread to finalize the progress meter
        /// </summary>
        /// <remarks>You must have called one of the Begin() methods prior to this call.</remarks>
        void End();
    }
}
