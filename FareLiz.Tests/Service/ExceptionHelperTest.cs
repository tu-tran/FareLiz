namespace FareLiz.Tests.Service
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SkyDean.FareLiz.Service;
    using System;
    using System.Windows.Forms;

    using SkyDean.FareLiz.Core.Utils;

    /// <summary>
    /// The exception helper test.
    /// </summary>
    [TestClass]
    public class ExceptionHelperTest
    {
        /// <summary>
        /// The test method 1.
        /// </summary>
        /// <exception cref="ApplicationException">
        /// </exception>
        [TestMethod]
        public void TestMethod1()
        {
            var logger = LogUtil.GetLogger("Test");
            var target = new ExceptionHelper(logger, true, false);
            Application.ThreadException += target.UnhandledThreadExceptionHandler;
            AppDomain.CurrentDomain.UnhandledException += target.UnhandledExceptionHandler;

            throw new ApplicationException("TEST");
        }
    }
}