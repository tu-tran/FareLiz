namespace FareLiz.Tests.Service
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SkyDean.FareLiz.Service.Utils;

    /// <summary>The process utils test.</summary>
    [TestClass]
    public class ProcessUtilsTest
    {
        /// <summary>The kill process method test.</summary>
        [TestMethod]
        public void KillProcessMethodTest()
        {
            ProcessUtils.KillProcess(@"D:\Personal\Projects\FareLiz\Debug\FareLiz.exe", 3, 3);
        }
    }
}