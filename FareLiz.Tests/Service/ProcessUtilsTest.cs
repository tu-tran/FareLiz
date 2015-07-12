using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkyDean.FareLiz.Service.Utils;

namespace FareLiz.Tests.Service
{
    [TestClass]
    public class ProcessUtilsTest
    {
        [TestMethod]
        public void KillProcessMethodTest()
        {
            ProcessUtils.KillProcess(@"D:\Personal\Projects\FareLiz\Debug\FareLiz.exe", 3, 3);
        }
    }
}
