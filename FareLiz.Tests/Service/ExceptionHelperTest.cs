using System;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkyDean.FareLiz.Service;

namespace FareLiz.Tests.Service
{
    [TestClass]
    public class ExceptionHelperTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var logger = log4net.LogManager.GetLogger("Test");
            var target = new ExceptionHelper(logger, true, false);
            Application.ThreadException += target.UnhandledThreadExceptionHandler;
            AppDomain.CurrentDomain.UnhandledException += target.UnhandledExceptionHandler;

            throw new ApplicationException("TEST");
        }
    }
}
